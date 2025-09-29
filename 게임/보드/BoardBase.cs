using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace inonego
{
    using Serializable;
    
    // ============================================================
    /// <summary>
    /// 보드 공간을 표현하기 위한 추상 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public abstract class BoardSpace<TPlaceable> : IBoardSpace<TPlaceable>
    where TPlaceable : class, new()
    {
        [SerializeReference]
        protected TPlaceable placed;
        public TPlaceable Placed
        {
            get => placed;
            set => placed = value;
        }

        public bool IsFull => Placed != null;
    }

    // ============================================================
    /// <summary>
    /// 보드를 표현하기 위한 추상 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public abstract class BoardBase<TPoint, TBoardSpace, TPlaceable> : IBoard<TPoint, TBoardSpace, TPlaceable>, IEnumerable<TBoardSpace>, IEnumerable
    where TPoint : struct, IEquatable<TPoint>
    where TBoardSpace : BoardSpace<TPlaceable>, new()
    where TPlaceable : class, new()
    {

    #region 필드

        [SerializeField]
        protected XDictionary<TPoint, TBoardSpace> spaceMap = new();

        [SerializeField]
        protected XDictionary<SerializeReferenceWrapper<TPlaceable>, TPoint> pointMap = new();

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 좌표에 있는 공간을 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TBoardSpace this[TPoint point]
        {
            get
            {
                return spaceMap.TryGetValue(point, out var space) ? space : null;
            }
        }

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 객체가 있는 좌표를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPoint? this[TPlaceable placeable]
        {
            get
            {
                return pointMap.TryGetValue(placeable, out var point) ? point : null;
            }
        }

        //------------------------------------------------------------
        /// <summary>
        /// 모든 공간이 차있는지 확인합니다.
        /// </summary>
        //------------------------------------------------------------
        public bool IsAllSpaceFull => spaceMap.Values.All(space => space.IsFull);

    #endregion

    #region 인터페이스 구현

        public IEnumerator<TBoardSpace> GetEnumerator()
        {
            foreach (var kv in spaceMap)
            {
                yield return kv.Value;
            }
        }

        IEnumerator<TBoardSpace> IEnumerable<TBoardSpace>.GetEnumerator() => GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
        
    #region 초기 설정 및 초기화

        public BoardBase() {}

    #endregion

    #region 메서드

        /// ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치에 객체를 배치할 수 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool CanPlace(TPoint point, TPlaceable placeable = null)
        {
            TBoardSpace space = this[point];

            // 비어있거나 같은 기물이라면 배치할 수 있습니다.
            return space != null && (space.Placed == null || space.Placed == placeable);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치에 객체를 배치합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Place(TPoint point, TPlaceable placeable)
        {    
            if (placeable == null) return;

            if (!CanPlace(point, placeable))
            {
                #if UNITY_EDITOR
                    Debug.LogError($"보드 기물을 '{point}'에 배치할 수 없습니다. 유효한 위치를 설정하거나 먼저 배치된 기물을 제거해주세요.");
                #endif
                    
                return; 
            }

            // 원래 위치에서 기물을 제거합니다.
            Remove(placeable);

            TBoardSpace space = this[point];

            if (space == null) return;

            space.Placed = placeable;
            pointMap[placeable] = point;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 지정된 위치의 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Remove(TPoint point)
        {
            TBoardSpace space = this[point];

            if (space == null) return;

            var placeable = space.Placed;
            
            if (placeable == null) return;

            space.Placed = null;
            pointMap.Remove(placeable);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// 지정된 기물을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Remove(TPlaceable placeable)
        {
            if (placeable == null) return;

            var point = this[placeable];

            if (point.HasValue)
            {
                Remove(point.Value);
            }
        }
        
    #endregion

    }
}