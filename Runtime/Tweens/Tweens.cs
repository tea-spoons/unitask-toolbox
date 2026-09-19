
namespace TeaSpoons.UniTaskToolbox.Tweens
{
    using UnityEngine;
    using Cysharp.Threading.Tasks;
    using System;
    using System.Threading;

    /// <summary>
    /// Awaitable tweening functions.
    /// </summary>
    public static partial class Tweens
    {
        #region Update Functions
        public delegate (float deltaTime, YieldAwaitable waitTask) UpdateFunction();

        private static UpdateFunction defaultUpdate => Update;

        public static readonly UpdateFunction Update = () => (Time.deltaTime, UniTask.Yield());
        public static readonly UpdateFunction UpdateRealtime = () => (Time.unscaledDeltaTime, UniTask.Yield());
        public static readonly UpdateFunction LateUpdate = () => (Time.deltaTime, UniTask.Yield(PlayerLoopTiming.PostLateUpdate));
        public static readonly UpdateFunction FixedUpdate = () => (Time.fixedDeltaTime, UniTask.Yield(PlayerLoopTiming.FixedUpdate));
        #endregion

        #region Cancellation
        public readonly struct Cancellation
        {
            public readonly CancellationToken token;
            public readonly bool finishOnCancel; 

            public Cancellation(CancellationToken token, bool finishOnCancel)
            {
                this.token = token;
                this.finishOnCancel = finishOnCancel;
            }

            public static implicit operator Cancellation(CancellationToken token)
            {
                return new Cancellation(token, false);
            }

            public static Cancellation FinishOn(CancellationToken token)
            {
                return new Cancellation(token, true);
            }
        }
        #endregion

#if UNITY_EDITOR
        private static bool shouldCallback => Application.isPlaying;
#endif

        // Interpolate in a specific time.
        #region Interpolate
        public static async UniTask Interpolate(float from, float to,
            float duration,
            Action<float> callback,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(from, to, duration, callback, Mathf.LerpUnclamped, update, curve, cancellation);
        }

        public static async UniTask Interpolate(Vector2 from, Vector2 to,
            float duration,
            Action<Vector2> callback,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(from, to, duration, callback, Vector2.LerpUnclamped, update, curve, cancellation);
        }

        public static async UniTask Interpolate(Vector3 from, Vector3 to,
            float duration,
            Action<Vector3> callback,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(from, to, duration, callback, Vector3.LerpUnclamped, update, curve, cancellation);
        }

        public static async UniTask Interpolate(Quaternion from, Quaternion to,
            float duration,
            Action<Quaternion> callback,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(from, to, duration, callback, Quaternion.SlerpUnclamped, update, curve, cancellation);
        }

        public static async UniTask Interpolate(Color from, Color to,
            float duration,
            Action<Color> callback,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            await Interpolate(from, to, duration, callback, Color.LerpUnclamped, update, curve, cancellation);
        }

        public static async UniTask Interpolate<T>(T from, T to,
            float duration,
            Action<T> callback,
            Func<T, T, float, T> lerp,
            UpdateFunction update = null,
            AnimationCurve curve = null,
            Cancellation cancellation = default)
        {
            if (duration <= 0f)
            {
                callback(curve != null ? lerp(from, to, curve.Evaluate(1f)) : to);
                return;
            }

            update ??= defaultUpdate;

            var time = 0f;
            while (time < duration)
            {
                if (cancellation.token.IsCancellationRequested)
                {
                    if (cancellation.finishOnCancel)
                    {
                        try
                        {
                            callback(curve != null ? lerp(from, to, curve.Evaluate(1f)) : to);
                        }
                        // Fail silently (in the player), as this can happen during/right after OnDestroy
                        // Throw in the editor because it can be a pain to track down sometimes otherwise
                        catch (NullReferenceException e)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"Caught NRE during tween cancellation: {e.Message}");
#endif
                        }
                        catch (MissingReferenceException e)
                        {
#if UNITY_EDITOR
                            Debug.LogError($"Tween MissingReference on cancel: {e.Message}");
#endif
                        }
                    }
                    return;
                }

                (var deltaTime, var wait) = update();

                time += deltaTime;

                if (time < duration)
                {
                    var t = time / duration;
                    if (curve != null)
                    {
                        t = curve.Evaluate(t);
                    }
                    var value = lerp(from, to, t);
#if UNITY_EDITOR
                    if (shouldCallback)
                    {
                        callback(value);
                    }
#else
                    callback(value);
#endif

                    await wait;
                }
                else
                {
#if UNITY_EDITOR
                    if (shouldCallback)
                    {
                        callback(curve != null ? lerp(from, to, curve.Evaluate(1f)) : to);
                    }
#else
                    callback(curve != null ? lerp(from, to, curve.Evaluate(1f)) : to);
#endif
                    return;
                }
            }
        }
        #endregion

        // Interpolate with a specific speed.
        #region MoveTowards
        public static async UniTask MoveTowards(float from, float to,
            float speed,
            Action<float> callback,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(from, to, speed, callback, Mathf.MoveTowards, update, cancellation);
        }

        public static async UniTask MoveTowards(Vector2 from, Vector2 to,
            float speed,
            Action<Vector2> callback,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(from, to, speed, callback, Vector2.MoveTowards, update, cancellation);
        }

        public static async UniTask MoveTowards(Vector3 from, Vector3 to,
            float speed,
            Action<Vector3> callback,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(from, to, speed, callback, Vector3.MoveTowards, update, cancellation);
        }

        public static async UniTask MoveTowards(Quaternion from, Quaternion to,
            float speed,
            Action<Quaternion> callback,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(from, to, speed, callback, Quaternion.RotateTowards, update, cancellation);
        }

        public static async UniTask MoveTowards(Color from, Color to,
            float speed,
            Action<Color> callback,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            await MoveTowards(from, to, speed, callback, (c1, c2, speed) => Vector4.MoveTowards(c1, c2, speed), update, cancellation);
        }

        public static async UniTask MoveTowards<T>(T from, T to,
            float speed,
            Action<T> callback,
            Func<T, T, float, T> moveTowards,
            UpdateFunction update = null,
            Cancellation cancellation = default)
        {
            if (speed <= 0f)
            {
                return;
            }

            if (update == null)
            {
                update = defaultUpdate;
            }

            var value = from;
            while (!Equals(value, to))
            {
                if (cancellation.token.IsCancellationRequested)
                {
                    if (cancellation.finishOnCancel)
                    {
                        try
                        {
                            callback(to);
                        }
                        // Fail silently on NREs, as this can happen during/right after OnDestroy
                        catch (NullReferenceException) { }
                        catch (MissingReferenceException) { }
                    }
                    return;
                }

                (var deltaTime, var wait) = update();

                value = moveTowards(value, to, speed * deltaTime);
#if UNITY_EDITOR
                if (shouldCallback)
                {
                    callback(value);
                }
#else
                callback(value);
#endif

                if (!Equals(value, to))
                {
                    await wait;
                }
            }
        }
        #endregion
    }
}
