using System;

using UnityEngine;

namespace inonego
{
    using Serializable;

    // ============================================================
    /// <summary>
    /// 값에 범위를 두고 관리하는 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class RangeValue<T> : Value<T>, IReadOnlyRangeValue<T>, IDeepCloneable<RangeValue<T>> where T : struct, IComparable<T>
    {

    #region 필드

        IReadOnlyValue<MinMax<T>> IReadOnlyRangeValue<T>.Range => range;

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 제한하는 범위입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        protected Value<MinMax<T>> range = new();
        public Value<MinMax<T>> Range => range;

        // ------------------------------------------------------------
        /// <summary>
        /// 최소값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Min => range.Current.Min;

        // ------------------------------------------------------------
        /// <summary>
        /// 최대값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Max => range.Current.Max;

    #endregion

    #region 생성자

        public RangeValue() : this(default, (default, default)) {}

        public RangeValue(T current, MinMax<T> range)
        {  
            this.range.Current = range;
            
            this.current = current;
            ProcessValue(default, ref base.current);
            
            this.range.OnCurrentChange += OnRangeChange;
        }

    #endregion

    #region 이벤트 핸들러

        private void OnRangeChange(object sender, ValueChangeEventArgs<MinMax<T>> e)
        {
            // 값을 다시 적용합니다.
            Current = Current;
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 값을 설정하기 전에 처리하는 메서드입니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override void ProcessValue(in T prev, ref T next)
        {
            next = range.Current.Clamp(next);
        }

    #endregion

    #region 복제

        public new RangeValue<T> @new() => new RangeValue<T>();

        public new RangeValue<T> Clone()
        {
            var result = @new();
            result.CloneFrom(this);
            return result;
        }

        public void CloneFrom(RangeValue<T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException($"RangeValue<T>.CloneFrom()의 인자가 null입니다.");
            }

            base.CloneFrom(source);

            range.CloneFrom(source.range);
        }

    #endregion

    #region 암시적 변환

        public int CompareTo(T other) => current.CompareTo(other);

        // ------------------------------------------------------------
        /// <summary>
        /// MinMaxValue<T>에서 T로의 암시적 변환입니다.
        /// </summary>
        // ------------------------------------------------------------
        public static implicit operator T(RangeValue<T> wrapper)
        {
            return wrapper != null ? wrapper.current : default;
        }

    #endregion

    #region Object 오버라이드

        public override bool Equals(object obj)
        {
            if (obj is RangeValue<T> other)
                return Equals(current, other.current);
            if (obj is T directValue)
                return Equals(current, directValue);
            return false;
        }

        public override int GetHashCode() => current.GetHashCode();

        public override string ToString() => $"{current} {range.Current}";

    #endregion

    }


}