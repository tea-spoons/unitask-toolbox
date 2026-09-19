
namespace TeaSpoons.UniTaskToolbox.Editor.Tests
{
    using NUnit.Framework;
    using UnityEngine.TestTools;
    using System.Threading.Tasks;
    using Cysharp.Threading.Tasks;
    using System.Diagnostics;
    using System.Text.RegularExpressions;
    using System;
    using System.Threading;

    public class AwaitableEventTest
    {
        private const string exceptionText = "Test Exception";
        private AwaitableEvent evt;
        private Stopwatch stopwatch;

        [SetUp]
        public void SetUp()
        {
            evt = new AwaitableEvent();
            stopwatch = new Stopwatch();
        }

        [TearDown]
        public void TearDown()
        {
        }

        [Test]
        public async Task NoResponses()
        {
            await evt.Invoke();
        }

        [Test]
        public async Task OnlyInstantResponse()
        {
            var success = false;
            evt.AddAction(() => success = true);

            await evt.Invoke();

            Assert.IsTrue(success);
        }

        [Test]
        public async Task AwaitSingle()
        {
            evt.AddTask(_ => UniTask.Delay(500));

            var elapsed = await InvokeEventAndMeasureTime();

            Assert.GreaterOrEqual(elapsed, 500);
        }

        [Test]
        public async Task AwaitMultiple()
        {
            evt.AddTask(_ => UniTask.Delay(200));
            evt.AddTask(_ => UniTask.Delay(200));

            var elapsed = await InvokeEventAndMeasureTime();

            Assert.GreaterOrEqual(elapsed, 200);
            // Make sure the tasks are running in parallel, not in sequence
            Assert.Less(elapsed, 400);
        }

        [Test]
        public async Task AwaitWithInstantException()
        {
            var waited = false;
            evt.AddTask(async _ =>
            {
                await UniTask.Delay(100);
                waited = true;
            });
            evt.AddAction(() => throw new System.Exception(exceptionText));

            LogAssert.Expect(new Regex($".*{exceptionText}.*"));
            await evt.Invoke();

            Assert.IsTrue(waited);
        }

        [Test]
        public async Task AwaitWithDelayedException()
        {
            var waited = false;
            evt.AddTask(async _ =>
            {
                await UniTask.Delay(10);
                throw new System.Exception(exceptionText);
            });
            evt.AddTask(async _ =>
            {
                await UniTask.Delay(100);
                waited = true;
            });

            LogAssert.Expect(new Regex($".*{exceptionText}.*"));
            await evt.Invoke();

            Assert.IsTrue(waited);
        }

        [Test]
        public async Task ManyResponses()
        {
            var missing = 4;

            Func<CancellationToken, UniTask> task = async _ =>
            {
                await UniTask.Delay(100);
                missing--;
            };

            evt.AddAction(() => missing--);
            evt.AddTask(task);
            evt.AddAction(() => missing--);
            evt.AddTask(task);

            await evt.Invoke();

            Assert.AreEqual(0, missing);
        }

        [Test]
        public async Task RemoveResponse()
        {
            var missing = 1;

            Func<CancellationToken, UniTask> task = async _ =>
            {
                await UniTask.Delay(100);
                missing--;
            };

            evt.AddTask(task);
            evt.AddTask(task);
            evt.RemoveTask(task);

            await evt.Invoke();

            Assert.AreEqual(0, missing);
        }

        [Test]
        public async Task CancelInvocation()
        {
            using var cancel = new CancellationTokenSource();
            var success = true;

            evt.AddTask(async cancel =>
            {
                await UniTask.Delay(200,
                    cancellationToken: cancel);
                success = false;
            });
            evt.AddTask(async _ =>
            {
                await UniTask.Delay(10);
                cancel.Cancel();
            });

            await evt.Invoke(cancel.Token);

            await UniTask.Delay(200);
            Assert.IsTrue(success);
        }

        [Test]
        public async Task InvokeMultipleTimes()
        {
            var missing = 4;

            evt.AddTask(async _ =>
            {
                await UniTask.Delay(100);
                missing--;
            });
            evt.AddTask(async _ =>
            {
                await UniTask.Delay(200);
                missing--;
            });

            await UniTask.WhenAll(evt.Invoke(), evt.Invoke());

            Assert.AreEqual(0, missing);
        }

        private async UniTask<long> InvokeEventAndMeasureTime()
        {
            stopwatch.Restart();
            await evt.Invoke();
            stopwatch.Stop();

            return stopwatch.ElapsedMilliseconds;
        }
    }
}
