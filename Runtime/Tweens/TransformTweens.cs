
namespace TeaSpoons.UniTaskToolbox.Tweens
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System;

    /// <summary>
    /// Transform-specific interpolation methods and callbacks.
    /// </summary>
    public static partial class Tweens
    {
        #region Interpolation
        public static async UniTask Move(Transform transform, Vector3 to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.position, to, duration, Move(transform), update, curve, cancellation);
        }

        public static async UniTask LocalMove(Transform transform, Vector3 to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.localPosition, to, duration, MoveLocal(transform), update, curve, cancellation);
        }

        public static async UniTask Rotate(Transform transform, Quaternion to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.rotation, to, duration, Rotate(transform), update, curve, cancellation);
        }

        public static async UniTask LocalRotate(Transform transform, Quaternion to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.localRotation, to, duration, RotateLocal(transform), update, curve, cancellation);
        }

        public static async UniTask Scale(Transform transform, Vector3 to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.localScale, to, duration, Scale(transform), update, curve, cancellation);
        }
        #endregion

        #region MoveTowards
        public static async UniTask MoveTowards(Transform transform, Vector3 to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.position, to, speed, Move(transform), update, cancellation);
        }

        public static async UniTask LocalMoveTowards(Transform transform, Vector3 to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.localPosition, to, speed, MoveLocal(transform), update, cancellation);
        }

        public static async UniTask RotateTowards(Transform transform, Quaternion to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.rotation, to, speed, Rotate(transform), update, cancellation);
        }

        public static async UniTask LocalRotateTowards(Transform transform, Quaternion to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.localRotation, to, speed, RotateLocal(transform), update, cancellation);
        }

        public static async UniTask ScaleTowards(Transform transform, Vector3 to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.localScale, to, speed, Scale(transform), update, cancellation);
        }
        #endregion

        #region Transform Callbacks
        public static Action<Vector3> Move(Transform transform)
        {
            return v => transform.position = v;
        }

        public static Action<Vector3> MoveLocal(Transform transform)
        {
            return v => transform.localPosition = v;
        }

        public static Action<Quaternion> Rotate(Transform transform)
        {
            return q => transform.rotation = q;
        }

        public static Action<Quaternion> RotateLocal(Transform transform)
        {
            return q => transform.localRotation = q;
        }

        public static Action<Vector3> Scale(Transform transform)
        {
            return v => transform.localScale = v;
        }
        #endregion
    }
}
