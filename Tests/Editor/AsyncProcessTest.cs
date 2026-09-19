
namespace TeaSpoons.UniTaskToolbox.Editor.Tests
{
    using Cysharp.Threading.Tasks;
    using NUnit.Framework;
    using NUnit.Framework.Internal;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncProcessTest
    {
        [Test]
        public async Task TestAsyncProcess()
        {
            var process = new AsyncProcess(out var trigger);

            var started = 0;
            var finished = 0;

            process.Starting += () => started++;
            process.Finished += () => finished++;

            Assert.AreEqual(0, started);
            Assert.AreEqual(0, finished);

            var wait = trigger.RunAsync(() => UniTask.WaitForSeconds(0.5f));
            Assert.AreEqual(1, started);
            Assert.AreEqual(0, finished);

            await wait;
            Assert.AreEqual(1, started);
            Assert.AreEqual(1, finished);

            wait = trigger.RunAsync(cancel => UniTask.WaitForSeconds(10f, cancellationToken: cancel));
            Assert.AreEqual(2, started);
            Assert.AreEqual(1, finished);

            await UniTask.WaitForSeconds(0.1f);
            Assert.AreEqual(2, started);
            Assert.AreEqual(1, finished);

            process.Cancel();
            await wait;
            Assert.AreEqual(2, started);
            Assert.AreEqual(2, finished);
        }

        [Test]
        public async Task TestAsyncProcessT()
        {
            var process = new AsyncProcess<int>(out var trigger);

            var started = 0;
            var finished = 0;
            var result = 0;

            process.Starting += () => started++;
            process.Finished += r =>
            {
                finished++;
                result = r;
            };

            Assert.AreEqual(0, started);
            Assert.AreEqual(0, finished);
            Assert.AreEqual(0, result);

            var wait = trigger.RunAsync(() => WaitAndReturn(0.5f, 10));
            Assert.AreEqual(1, started);
            Assert.AreEqual(0, finished);
            Assert.AreEqual(0, result);

            var waitResult = await wait;
            Assert.AreEqual(1, started);
            Assert.AreEqual(1, finished);
            Assert.AreEqual(10, result);
            Assert.AreEqual(10, waitResult);

            wait = trigger.RunAsync(cancel => WaitAndReturn(10f, 20, cancel));
            Assert.AreEqual(2, started);
            Assert.AreEqual(1, finished);
            Assert.AreEqual(10, result);
            Assert.AreEqual(10, waitResult);

            await UniTask.WaitForSeconds(0.1f);
            Assert.AreEqual(2, started);
            Assert.AreEqual(1, finished);
            Assert.AreEqual(10, result);
            Assert.AreEqual(10, waitResult);

            process.Cancel();
            waitResult = await wait;
            Assert.AreEqual(2, started);
            Assert.AreEqual(2, finished);
            Assert.AreEqual(20, result);
            Assert.AreEqual(20, waitResult);
        }

        private async UniTask<int> WaitAndReturn(float delay, int result)
        {
            await UniTask.WaitForSeconds(delay);
            return result;
        }

        private async UniTask<int> WaitAndReturn(float delay, int result, CancellationToken cancel)
        {
            await UniTask.WaitForSeconds(delay, cancellationToken: cancel);
            return result;
        }
    }
}
