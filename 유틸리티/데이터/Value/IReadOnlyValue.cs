using System;

using UnityEngine;

namespace inonego
{
    public interface IReadOnlyValue<T> where T : struct
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool InvokeEvent { get; set; }
        
        // ------------------------------------------------------------
        /// <summary>
        /// 현재 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Current { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEvent<IReadOnlyValue<T>, T> OnValueChange;
    }
}
