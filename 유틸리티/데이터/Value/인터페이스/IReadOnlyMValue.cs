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
        /// 수정자 적용 전의 기본 값입니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Base { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 수정자 목록입니다.
        /// </summary>
        // ------------------------------------------------------------
        public IReadOnlyList<IModifier<T>> Modifiers { get; }
    }
}
