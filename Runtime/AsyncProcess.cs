
namespace TeaSpoons.UniTaskToolbox
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Threading;

    /// <summary>
    /// Represents an async process with callback events when started and finished.
    /// </summary>
    public class AsyncProcess
    {
        public readonly struct Trigger
        {
            private readonly AsyncProcess target;

            internal Trigger(AsyncProcess target)
            {
                this.target = target;
            }

            public async UniTask RunAsync(Func<CancellationToken, UniTask> process)
            {
                target.Cancel();

                target.Starting();
                await process(target.cancel.Token);
                target.Finished();
            }

            public async UniTask RunAsync(Func<UniTask> process)
            {
                await RunAsync(token => process().AttachExternalCancellation(token));
            }
        }

        private readonly ReusableCancelSource cancel = new();

        public event Action Starting = delegate { };
        public event Action Finished = delegate { };

        public AsyncProcess(out Trigger trigger)
        {
            trigger = new(this);
        }

        public void Cancel()
        {
            cancel.Cancel();
        }
    }

    public class AsyncProcess<T>
    {
        public readonly struct Trigger
        {
            private readonly AsyncProcess<T> target;

            internal Trigger(AsyncProcess<T> target)
            {
                this.target = target;
            }

            public async UniTask<T> RunAsync(Func<CancellationToken, UniTask<T>> process)
            {
                target.Cancel();

                target.Starting();
                var result = await process(target.cancel.Token);
                target.Finished(result);

                return result;
            }

            public async UniTask<T> RunAsync(Func<UniTask<T>> process)
            {
                return await RunAsync(token => process().AttachExternalCancellation(token));
            }
        }

        private readonly ReusableCancelSource cancel = new();

        public event Action Starting = delegate { };
        public event Action<T> Finished = delegate { };

        public AsyncProcess(out Trigger trigger)
        {
            trigger = new(this);
        }

        public void Cancel()
        {
            cancel.Cancel();
        }
    }
}
