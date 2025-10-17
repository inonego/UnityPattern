using System;

#if DOTWEEN
using DG.Tweening;
#endif

namespace inonego
{
    
#if !DOTWEEN
    public enum Ease { None }
#endif

    [Serializable]
    public struct TweenInOutCurve
    {
        public TweenCurve In;
        public TweenCurve Out;

        public TweenInOutCurve(TweenCurve inCurve, TweenCurve outCurve)
        {
            (In, Out) = (inCurve, outCurve);
        }
    }

    [Serializable]
    public struct TweenCurve
    {
        public float Duration;
        public float Delay;
        public Ease Ease;

        public TweenCurve(float duration, float delay, Ease ease)
        {
            (Duration, Delay, Ease) = (duration, delay, ease);
        }
    }
}