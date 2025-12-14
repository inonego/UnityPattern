using System;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 보드를 나타내는 인터페이스입니다.
    /// </summary>
    // ============================================================
    public interface IBoard<TVector, TIndex, TPlaceable>
    where TVector : struct where TIndex : struct
    where TPlaceable : class
    {

    #region 공간

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 벡터에 있는 공간을 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public IBoardSpace<TIndex, TPlaceable> this[TVector vector] { get; }

    #endregion

    #region 객체

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 벡터와 인덱스에 있는 객체를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPlaceable this[TVector vector, TIndex index] { get; }

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 포인트에 있는 객체를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPlaceable this[IBoardPoint<TVector, TIndex> point] { get; }
   
    #endregion

    #region 좌표

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 객체의 위치를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public IBoardPoint<TVector, TIndex> this[TPlaceable placeable] { get; }

    #endregion
   
    }
}