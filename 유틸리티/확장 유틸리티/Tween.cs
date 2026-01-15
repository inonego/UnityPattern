#if DOTWEEN

using System;

using DG.Tweening;

namespace inonego
{
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

        public void Deconstruct(out float duration, out float delay, out Ease ease)
        {
            (duration, delay, ease) = (Duration, Delay, Ease);
        }
    }
}

#endif