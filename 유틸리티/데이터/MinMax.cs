using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct MinMax<T> where T : struct, IComparable<T>
    {

        [SerializeField]
        private T min;
        public T Min
        {
            get => min;
            set
            {
                if (value.CompareTo(max) > 0)
                {
                    throw new InvalidOperationException($"잘못된 범위입니다. ({value} - {max})");
                }

                min = value;
            }
        }

        [SerializeField]
        private T max;
        public T Max
        {
            get => max;
            set
            {
                if (value.CompareTo(min) < 0)
                {
                    throw new InvalidOperationException($"잘못된 범위입니다. ({min} - {value})");
                }

                max = value;
            }
        }

        public MinMax(T min, T max)
        {
            if (min.CompareTo(max) > 0)
            {
                throw new InvalidOperationException($"잘못된 범위입니다. ({min} - {max})");
            }

            this.min = min;
            this.max = max;
        }

        public T Clamp(T value)
        {
            if (value.CompareTo(Min) < 0) return Min;
            if (value.CompareTo(Max) > 0) return Max;

            return value;
        }

        public override bool Equals(object obj)
        {
            if (obj is MinMax<T> other)
            {
                return Min.Equals(other.Min) && Max.Equals(other.Max);  
            }
            
            return false;
        }

        public static implicit operator MinMax<T>((T Min, T Max) lTuple) => new(lTuple.Min, lTuple.Max);
        public static implicit operator (T Min, T Max)(MinMax<T> minMax) => (minMax.Min, minMax.Max);

        public override string ToString() => $"({Min} - {Max})";

        public override int GetHashCode() => HashCode.Combine(Min, Max);
    }
}