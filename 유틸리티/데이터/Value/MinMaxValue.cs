using System;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 최소값과 최대값 범위를 가지는 값을 관리하는 클래스입니다.
    /// 현재값이 항상 최소값과 최대값 사이에 유지되도록 합니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class MinMaxValue<T> : Value<T>, IReadOnlyMinMaxValue<T>, IComparable<T> where T : struct, IComparable<T>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 범위가 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEvent<MinMaxValue<T>, MinMax<T>> OnRangeChange = null;

        event ValueChangeEvent<IReadOnlyMinMaxValue<T>, MinMax<T>> IReadOnlyMinMaxValue<T>.OnRangeChange
        { add => OnRangeChange += value; remove => OnRangeChange -= value; }

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
                var (prev, next) = (this.range, value);

                ProcessRange(prev, ref next);

                // 범위 변경이 없으면 종료합니다.
                if (Equals(prev, next)) return;

                this.range = next;

                if (InvokeEvent)
                {
                    OnRangeChange?.Invoke(this, new() { Previous = prev, Current = next });
                }

                // 값이 범위를 벗어나면 조정될 수 있도록 재설정 합니다.
                Current = Current;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 최소값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Min 
        {
            get => Range.Min;
            set => Range = new MinMax<T>(value, Max);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 최대값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Max
        {
            get => Range.Max;
            set => Range = new MinMax<T>(Min, value);
        }

    #endregion

    #region 생성자

        public MinMaxValue()
        {
            range = default;
            current = default;

            ProcessRange(default, ref range);
            ProcessValue(default, ref current);
        }

        public MinMaxValue(MinMax<T> range, T value)
        {  
            this.range = range;
            this.current = value;

            ProcessRange(default, ref range);
            ProcessValue(default, ref current);
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 최소값과 최대값 사이로 제한합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected T ClampValue(T val)
        {
            if (val.CompareTo(range.Min) < 0) return range.Min;
            if (val.CompareTo(range.Max) > 0) return range.Max;

            return val;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 범위를 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected virtual void ProcessRange(in MinMax<T> prev, ref MinMax<T> next)
        {  
            // prev는 이미 완전무결하다고 가정하고 next만 처리합니다.
            if (next.Min.CompareTo(next.Max) > 0)
            {
                throw new ArgumentException("최소값이 최대값보다 클 수 없습니다.");
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override void ProcessValue(in T prev, ref T next)
        {
            next = ClampValue(next);
        }

    #endregion

    #region 암시적 변환

        public int CompareTo(T other) => current.CompareTo(other);

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