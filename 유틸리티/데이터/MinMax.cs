using System;

namespace inonego
{
    [Serializable]
    public struct MinMax<T> where T : struct, IComparable<T>
    {
        public T Min;
        public T Max;

        public MinMax(T min, T max) => (Min, Max) = (min, max);

        public T Clamp(T value)
        {
            if (value.CompareTo(Max) > 0) return Max;
            if (value.CompareTo(Min) < 0) return Min;

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