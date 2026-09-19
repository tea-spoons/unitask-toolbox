
namespace TeaSpoons.UniTaskToolbox
{
    using Cysharp.Threading.Tasks;
    using System;
    using System.Threading;
    using UnityEngine;

    /// <summary>
    /// A UniTask-based state machine class.
    /// Each state is represented by an async method that can call <see cref="SetNextState"/> for a state transition once it's finished.
    /// </summary>
    public sealed class StateMachine : IDisposable
    {
        public bool Running { get; private set; }

        private CancellationToken destroyToken;
        private CancellationTokenSource exit;
        /// <summary>
        /// A linked CTS, combining <see cref="exit"/> and <see cref="destroyToken"/>.
        /// </summary>
        private CancellationTokenSource cancel;
        private Func<UniTask> nextState;
        private Action nextInstantState;
        private bool hasNextState => nextState != null || nextInstantState != null;
        private bool isDisposed = false;

        public StateMachine(MonoBehaviour owner)
        {
#if UNITY_2022_1_OR_NEWER
            destroyToken = owner.destroyCancellationToken;
#else
            destroyToken = owner.GetCancellationTokenOnDestroy();
#endif
        }

        /// <summary>
        /// Starts the machine with the given <paramref name="state"/>.
        /// </summary>
        /// <remarks>
        /// The machine must not be running when calling this.
        /// </remarks>
        public void Start(Func<UniTask> state)
        {
            if (Running || isDisposed) return;

            EnsureFreshCancellationTokenSource();

            nextState = state;
            nextInstantState = null;
            Run().Forget(LogException);
        }

        /// <summary>
        /// Starts the machine with the given <paramref name="state"/>.
        /// </summary>
        /// <remarks>
        /// The machine must not be running when calling this.
        /// </remarks>
        public void Start(Func<CancellationToken, UniTask> state)
        {
            Start(() => state(cancel.Token));
        }

        /// <summary>
        /// Starts the machine with the given <paramref name="state"/>.
        /// </summary>
        /// <remarks>
        /// The machine must not be running when calling this.
        /// </remarks>
        public void Start(Action state)
        {
            if (Running || isDisposed) return;

            EnsureFreshCancellationTokenSource();

            nextState = null;
            nextInstantState = state;
            Run().Forget(LogException);
        }

        /// <summary>
        /// Sets the given <paramref name="state"/> as the next state.
        /// </summary>
        /// <remarks>
        /// If the state machine is already in another state, that state needs to finish first.
        /// Will not work if the machine is currently cancelling.
        /// </remarks>
        public void SetNextState(Func<UniTask> state)
        {
            if (!Running || cancel.IsCancellationRequested) return;

            nextState = state;
            nextInstantState = null;
        }

        /// <summary>
        /// Sets the given <paramref name="state"/> as the next state.
        /// </summary>
        /// <remarks>
        /// If the state machine is already in another state, that state needs to finish first.
        /// Will not work if the machine is currently cancelling or not running.
        /// </remarks>
        public void SetNextState(Func<CancellationToken, UniTask> state)
        {
            if (!Running || cancel.IsCancellationRequested) return;

            SetNextState(() => state(cancel.Token));
        }

        /// <summary>
        /// Sets the given <paramref name="state"/> as the next state.
        /// </summary>
        /// <remarks>
        /// If the state machine is already in another state, that state needs to finish first.
        /// Will not work if the machine is currently cancelling or not running.
        /// </remarks>
        public void SetNextState(Action state)
        {
            if (!Running || cancel.IsCancellationRequested) return;

            nextState = null;
            nextInstantState = state;
        }

        public void Exit()
        {
            exit?.Cancel();
        }

        private async UniTask Run()
        {
            Running = true;

            while (hasNextState &&
                !cancel.IsCancellationRequested &&
                !isDisposed)
            {
                if (nextState != null)
                {
                    var state = nextState;
                    nextState = null;
                    await state();
                }
                else
                {
                    var state = nextInstantState;
                    nextInstantState = null;
                    state();
                }
            }

            nextState = null;
            nextInstantState = null;
            Running = false;
        }

        private void EnsureFreshCancellationTokenSource()
        {
            if (exit == null || exit.IsCancellationRequested)
            {
                exit?.Dispose();
                exit = new();
                cancel = CancellationTokenSource.CreateLinkedTokenSource(destroyToken, exit.Token);
            }
        }

        private void LogException(Exception exception)
        {
            if (exception is not OperationCanceledException)
            {
                Debug.LogException(exception);
            }
        }

        public void Dispose()
        {
            Exit();
            exit?.Dispose();
            cancel?.Dispose();
            isDisposed = true;
        }
    }
}
