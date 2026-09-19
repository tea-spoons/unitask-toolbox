
namespace TeaSpoons.UniTaskToolbox.Tweens
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System;

    /// <summary>
    /// RectTransform-specific interpolation methods and callbacks.
    /// </summary>
    public static partial class Tweens
    {
        #region Interpolation
        public static async UniTask AnchorMove(RectTransform transform, Vector2 to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.anchoredPosition, to, duration, AnchorMove(transform), update, curve, cancellation);
        }

        public static async UniTask ScaleWithAnchors(RectTransform transform, Vector2 to,
            float duration,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(transform.rect.size, to, duration, SetSizeWithCurrentAnchors(transform), update, curve, cancellation);
        }
        #endregion

        #region MoveTowards
        public static async UniTask MoveTowards(RectTransform transform, Vector2 to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.anchoredPosition, to, speed, AnchorMove(transform), update, cancellation);
        }

        public static async UniTask SetSizeTowardsWithCurrentAnchors(RectTransform transform, Vector2 to,
            float speed,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(transform.rect.size, to, speed, SetSizeWithCurrentAnchors(transform), update, cancellation);
        }
        #endregion

        #region Transform Callbacks
        public static Action<Vector2> AnchorMove(RectTransform transform)
        {
            return v => transform.anchoredPosition = v;
        }

        public static Action<Vector2> SetSizeWithCurrentAnchors(RectTransform transform)
        {
            return v =>
            {
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, v.x);
                transform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, v.y);
            };
        }
        #endregion
    }
}
