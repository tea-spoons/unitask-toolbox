
namespace TeaSpoons.UniTaskToolbox
{
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityObject = UnityEngine.Object;

    public static class AsyncInstantiateOperationExtensions
    {
        /// <summary>
        /// Gets the first result of an <see cref="AsyncInstantiateOperation"/>.
        /// Returns <c>null</c> when the operation produced no instance, for example because it was
        /// cancelled or its target was destroyed before the integration stage.
        /// </summary>
        public static async UniTask<T> GetFirst<T>(this AsyncInstantiateOperation<T> operation)
            where T : UnityObject
        {
            await operation;
            if (operation.isDone)
            {
                var results = operation.Result;
                return results is { Length: > 0 } ? results[0] : null;
            }
            return null;
        }
    }
}
