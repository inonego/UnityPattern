using System;

using UnityEngine;

namespace inonego
{
    public interface IReadOnlyMinMaxValue<T> : IReadOnlyValue<T> where T : struct, IComparable<T>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 최소와 최대의 범위입니다.
        /// </summary>
        // ------------------------------------------------------------
        public MinMax<T> Range { get; }
        
        // ------------------------------------------------------------
        /// <summary>
        /// 최소값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Min { get; }
        
        // ------------------------------------------------------------
        /// <summary>
        /// 최대값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Max { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 범위가 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEvent<IReadOnlyMinMaxValue<T>, MinMax<T>> OnRangeChange;
    }
}
