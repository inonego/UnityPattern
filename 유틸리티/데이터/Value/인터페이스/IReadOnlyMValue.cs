using System;
using System.Collections;
using System.Collections.Generic;

namespace inonego
{
    // =================================================================
    /// <summary>
    /// 읽기 전용 MValue 인터페이스입니다.
    /// </summary>
    // =================================================================
    public interface IReadOnlyMValue<T> : IReadOnlyValue<T> where T : struct
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 수정자가 적용된 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Modified { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자 목록입니다.
        /// </summary>
        // ------------------------------------------------------------
        public IReadOnlyList<IModifier<T>> Modifiers { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자가 적용된 값이 변경될 때 발생하는 이벤트입니다.
        /// </summary>
        // ------------------------------------------------------------
        public event ValueChangeEventHandler<T> OnModifiedChange;
    }
}
