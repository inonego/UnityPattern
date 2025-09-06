using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct MinMax
    {
        public float Min, Max;

        public MinMax(float min, float max)
        {
            (Min, Max) = (min, max);
        }
    }
}