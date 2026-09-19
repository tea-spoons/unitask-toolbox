
namespace TeaSpoons.UniTaskToolbox.Tweens
{
    using UnityEngine;

    public static class TweenCurves
    {
        public static AnimationCurve easeIn
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 0), 
                                          new Keyframe(1, 1, 2, 0));
            }
        }

        public static AnimationCurve easeInOut
        {
            get { return AnimationCurve.EaseInOut(0, 0, 1, 1); }
        }

        public static AnimationCurve easeOut
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 2),
                                          new Keyframe(1, 1, 0, 0));
            }
        }

        public static AnimationCurve backOut
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 4),
                                          new Keyframe(1, 1, 0, 0));
            }
        }

        public static AnimationCurve bounceOut
        {
            get
            {
                return new AnimationCurve(new Keyframe(0, 0, 0, 0),
                                          new Keyframe(0.4f, 1, 5, -4),
                                          new Keyframe(0.7f, 1, 4, -3),
                                          new Keyframe(0.9f, 1, 3, -2),
                                          new Keyframe(1, 1, 2, 0));
            }
        }
    }
}
