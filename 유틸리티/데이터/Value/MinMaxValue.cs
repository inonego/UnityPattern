using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public struct MinMax<T> where T : struct, IComparable<T>
    {
        public T Min, Max;

        public MinMax(T min, T max)
        {
            (Min, Max) = (min, max);
        }
    }

    // ============================================================
    /// <summary>
    /// 최소값과 최대값 범위를 가지는 값을 관리하는 클래스입니다.
    /// 현재값이 항상 최소값과 최대값 사이에 유지되도록 합니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class MinMaxValue<T> : Value<T> where T : struct, IComparable<T>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 범위가 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEvent<MinMaxValue<T>, MinMax<T>> OnRangeChange = null;

    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// 최소와 최대의 범위입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private MinMax<T> range;
        public MinMax<T> Range
        {
            get => range;
            set
            {
                var (prev, next) = (this.range, CheckRange(value));

                // 범위 변경이 없으면 종료합니다.
                if (Equals(prev, next)) return;

                this.range = next;

                if (InvokeEvent)
                {
                    OnRangeChange?.Invoke(this, new ValueChangeEventArgs<MinMax<T>> { Previous = prev, Current = next });
                }

                // 값이 범위를 벗어나면 조정될 수 있도록 재설정 합니다.
                Current = Current;
            }
        }

    #endregion

    #region 생성자

        public MinMaxValue()
        {
            range = default;
            current = default;
        }

        public MinMaxValue(MinMax<T> range, T value)
        {  
            this.range = CheckRange(range);
            this.current = ClampValue(value);
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 범위가 유효한지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        private MinMax<T> CheckRange(MinMax<T> range)
        {
            if (range.Min.CompareTo(range.Max) > 0)
            {
                throw new ArgumentException("최소값이 최대값보다 클 수 없습니다.");
            }
            
            return range;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 최소값과 최대값 사이로 제한합니다.
        /// </summary>
        // ------------------------------------------------------------
        private T ClampValue(T val)
        {
            if (val.CompareTo(range.Min) < 0) return range.Min;
            if (val.CompareTo(range.Max) > 0) return range.Max;

            return val;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override T Process(T value)
        {
            return ClampValue(value);
        }

    #endregion

    #region 암시적 변환

        // ------------------------------------------------------------
        /// <summary>
        /// MinMaxValue<T>에서 T로의 암시적 변환입니다.
        /// </summary>
        // ------------------------------------------------------------
        public static implicit operator T(MinMaxValue<T> wrapper)
        {
            return wrapper != null ? wrapper.current : default;
        }

    #endregion

    #region Object 오버라이드

        public override bool Equals(object obj)
        {
            if (obj is MinMaxValue<T> other)
                return Equals(current, other.current);
            if (obj is T directValue)
                return Equals(current, directValue);
            return false;
        }

        public override int GetHashCode() => current.GetHashCode();

        public override string ToString() => $"{current} ({range.Min} - {range.Max})";

    #endregion

    }


}