
namespace TeaSpoons.UniTaskToolbox
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;

    /// <summary>
    /// An event whose invocation can be awaited until all registered responses are finished.
    /// </summary>
    public class AwaitableEvent
    {
        public readonly struct ResponseProxy
        {
            private readonly AwaitableEvent owner;

            internal ResponseProxy(AwaitableEvent owner)
            {
                this.owner = owner;
            }

            public void AddTask(Func<CancellationToken, UniTask> task)
            {
                owner.AddTask(task);
            }

            public void AddAction(Action action)
            {
                owner.AddAction(action);
            }

            public void RemoveTask(Func<CancellationToken, UniTask> task)
            {
                owner.RemoveTask(task);
            }

            public void RemoveAction(Action action)
            {
                owner.RemoveAction(action);
            }
        }

        public readonly ResponseProxy Responses;

        private readonly List<Func<CancellationToken, UniTask>> tasks = new();
        private readonly List<Action> actions = new();
        /// <summary>
        /// This list is used during event invocation to store the running tasks.
        /// Every additional invocation requires a new list to be created, causing garbage.
        /// However, this list is used to remove garbage creation for the majority of cases, where the event is running only once at a time.
        /// </summary>
        private readonly List<UniTask> primaryInvocationTasks = new();


        public AwaitableEvent()
        {
            Responses = new ResponseProxy(this);
        }

        public void AddTask(Func<CancellationToken, UniTask> task)
        {
            tasks.Add(task);
        }

        public void AddAction(Action action)
        {
            actions.Add(action);
        }

        public void RemoveTask(Func<CancellationToken, UniTask> task)
        {
            tasks.Remove(task);
        }

        public void RemoveAction(Action action)
        {
            actions.Remove(action);
        }

        public async UniTask Invoke()
        {
            InvokeAllActions();
            await InvokeAllTasksAsync(task => task(default));
        }

        public async UniTask Invoke(CancellationToken cancellationToken)
        {
            InvokeAllActions();
            await InvokeAllTasksAsync(task => task(cancellationToken).SuppressCancellationThrow());
        }

        public void Clear()
        {
            actions.Clear();
            tasks.Clear();
        }

        /// <summary>
        /// Invokes all registered actions.
        /// </summary>
        /// <remarks>
        /// If an action throws an exception, <see cref="HandleException(Exception)"/> is called.
        /// Action invocation will continue, and the method will finish gracefully.
        /// </remarks>
        private void InvokeAllActions()
        {
            foreach (var action in actions)
            {
                try
                {
                    action();
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }
            }
        }

        /// <summary>
        /// Starts and awaits all registered tasks in parallel.
        /// </summary>
        /// <remarks>
        /// If a task throws an exception, <see cref="HandleException(Exception)"/> is called.
        /// All other tasks continue unaffected, and the method will finish gracefully.
        /// </remarks>
        private async UniTask InvokeAllTasksAsync(Func<Func<CancellationToken, UniTask>, UniTask> select)
        {
            if (tasks.Count == 0) return;

            var openTasks = MaterializeTasks(tasks.Select(select));

            foreach (var task in openTasks)
            {
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    HandleException(exception);
                }
            }

            openTasks.Clear();
        }

        /// <summary>
        /// Materializes the tasks so they start running.
        /// </summary>
        /// <remarks>
        /// Uses the <see cref="primaryInvocationTasks"/> list if it's currently empty to avoid creating garbage.
        /// If the event is invoked before the previous invocation has finished, a new list is created for this.
        /// </remarks>
        private List<UniTask> MaterializeTasks(IEnumerable<UniTask> selected)
        {
            List<UniTask> openTasks;
            if (primaryInvocationTasks.Count == 0)
            {
                openTasks = primaryInvocationTasks;
                openTasks.AddRange(selected);
            }
            else
            {
                openTasks = selected.ToList();
            }

            return openTasks;
        }

        private static void HandleException(Exception exception)
        {
            UnityEngine.Debug.LogException(exception);
        }
    }
}
