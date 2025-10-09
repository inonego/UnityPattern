using System;

namespace inonego
{
    [Serializable]
    public struct MinMax<T> where T : struct, IComparable<T>
    {
        public T Min;
        public T Max;

        public MinMax(T min, T max) => (Min, Max) = (min, max);

        public override bool Equals(object obj)
        {
            if (obj is MinMax<T> other)
            {
                return Min.Equals(other.Min) && Max.Equals(other.Max);  
            }
            
            return false;
        }

        public override int GetHashCode() => HashCode.Combine(Min, Max);
    }
}