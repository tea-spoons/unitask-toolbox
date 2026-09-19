namespace TeaSpoons.UniTaskToolbox
{
    using System;
    using System.Collections;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public static class CoroutineExtensions
    {
        private static CoroutineRunner runner;

        /// <summary>
        /// Bridges a coroutine that produces a result into a <see cref="UniTask{T}"/>.
        /// The coroutine receives a <see cref="Action{T}"/> resolve callback;
        /// call it at every exit point to set the result.
        /// </summary>
        /// <example>
        /// <code>
        /// private IEnumerator MyCoroutine(Action&lt;int&gt; resolve)
        /// {
        ///     yield return something;
        ///     resolve(42);
        /// }
        ///
        /// var result = await CoroutineExtensions.FromCoroutine&lt;int&gt;(MyCoroutine);
        /// </code>
        /// </example>
        public static UniTask<T> FromCoroutine<T>(Func<Action<T>, IEnumerator> coroutineFactory)
        {
            var completionSource = new UniTaskCompletionSource<T>();
            EnsureRunner().StartCoroutine(coroutineFactory(result => completionSource.TrySetResult(result)));
            return completionSource.Task;
        }

        private static CoroutineRunner EnsureRunner()
        {
            if (runner) return runner;

            var go = new GameObject("[UniTaskToolbox.CoroutineRunner]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            runner = go.AddComponent<CoroutineRunner>();
            return runner;
        }

        private class CoroutineRunner : MonoBehaviour { }
    }
}