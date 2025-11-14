using System;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 보드의 공간을 나타낸 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface IBoardSpace<TIndex, TPlaceable>
    where TIndex : struct
    where TPlaceable : class, new()
    {
        //------------------------------------------------------------
        /// <summary>
        /// 지정된 인덱스에 있는 객체를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPlaceable this[TIndex index] { get; }
    }
}