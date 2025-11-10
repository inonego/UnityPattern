using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 월드상에 게임 오브젝트로 존재할 수 있는 보드를 표현하기 위한 인터페이스입니다.
    /// </summary>
    // --------------------------------------------------------------------------------
    public interface IMonoBoard<TBoard, TPoint, TBoardSpace, TPlaceable>
    where TBoard : class, IBoard<TPoint, TBoardSpace, TPlaceable>
    where TPoint : struct
    where TBoardSpace : class, IBoardSpace<TPlaceable>, new()
    where TPlaceable : class, new()
    {
        public TBoard Board { get; }

        public IReadOnlyDictionary<TPoint, GameObject> TileMap { get; }

        public Vector3 ToPos(TPoint point);
    }
}