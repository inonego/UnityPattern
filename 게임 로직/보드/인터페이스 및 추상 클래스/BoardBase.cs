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
    public class BoardSpace<TPlaceable> : IBoardSpace<TPlaceable>
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

    [Serializable]
    public abstract partial class BoardBase
    {
        public BoardBase() {}
    }

    // ============================================================
    /// <summary>
    /// 보드를 표현하기 위한 추상 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public abstract class BoardBase<TPoint, TBoardSpace, TPlaceable> : BoardBase, IBoard<TPoint, TBoardSpace, TPlaceable>, IEnumerable<KeyValuePair<TPoint, TBoardSpace>>, IEnumerable
    where TPoint : struct
    where TBoardSpace : BoardSpace<TPlaceable>, new()
    where TPlaceable : class, new()
    {

    #region 필드

        [SerializeField]
        protected XDictionary<TPoint, TBoardSpace> spaceMap = new();

        [SerializeField]
        protected XDictionary<SerializeReferenceWrapper<TPlaceable>, TPoint> pointMap = new();

        protected virtual bool IsValidPoint(TPoint point) => true;

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 좌표에 있는 공간을 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TBoardSpace this[TPoint point]
        {
            get
            {
                if (!IsValidPoint(point)) return null;

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

    #region 이벤트

        public event Action<TPoint, TBoardSpace, TPlaceable> OnPlace = null;
        public event Action<TPoint, TBoardSpace, TPlaceable> OnRemove = null;

        public event Action<TPoint> OnAddSpace = null;
        public event Action<TPoint> OnRemoveSpace = null;

    #endregion

    #region 인터페이스 구현

        IEnumerator<KeyValuePair<TPoint, TBoardSpace>> IEnumerable<KeyValuePair<TPoint, TBoardSpace>>.GetEnumerator() => spaceMap.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => spaceMap.GetEnumerator();

    #endregion
        
    #region 초기 설정 및 초기화

        public BoardBase() {}

    #endregion

    #region 공간 생성 및 제거 메서드

        // ---------------------------------------------------------------
        /// <summary>
        /// 공간을 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual TBoardSpace CreateSpace() => new TBoardSpace();

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치에 공간을 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void AddSpace(TPoint point, bool invokeEvent = true)
        {
            if (!IsValidPoint(point)) return;

            // 이미 공간이 존재하면 중복 생성하지 않습니다.
            if (spaceMap.ContainsKey(point))
            {
                throw new SpaceAlreadyExistsException();
            }

            spaceMap[point] = CreateSpace();

            if (invokeEvent)
            {
                OnAddSpace?.Invoke(point);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치의 공간을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RemoveSpace(TPoint point, bool invokeEvent = true)
        {
            RemoveSpace(point, invokeEvent, removeFromMap: true);
        }

        protected virtual void RemoveSpace(TPoint point, bool invokeEvent = true, bool removeFromMap = true)
        {
            if (!IsValidPoint(point)) return;

            if (!spaceMap.TryGetValue(point, out var space)) return;

            // 공간에 있는 객체를 제거합니다.
            Remove(space.Placed);

            if (removeFromMap)
            {
            // 공간을 제거합니다.
                spaceMap.Remove(point);
            }

            if (invokeEvent)
            {
                OnRemoveSpace?.Invoke(point);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 모든 공간을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void RemoveSpaceAll()
        {
            foreach (var point in spaceMap.Keys)
            {
                RemoveSpace(point, removeFromMap: false);
            }

            spaceMap.Clear();
        }

    #endregion

    #region 배치 및 제거 메서드

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
            if (placeable == null) throw new ArgumentNullException("배치할 객체를 설정해주세요.");

            TBoardSpace space = this[point];
            
            if (space == null) 
            {
                throw new SpaceNotFoundException();
            }

            if (!CanPlace(point, placeable))
            {
                throw new InvalidPlacementException();
            }

            // 원래 위치에서 객체를 제거합니다.
            Remove(placeable);

            space.Placed = placeable;
            pointMap[placeable] = point;

            OnPlace?.Invoke(point, space, placeable);
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

            OnRemove?.Invoke(point, space, placeable);
        }

        /// ---------------------------------------------------------------
        /// <summary>
        /// 지정된 객체를 제거합니다.
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

        /// ---------------------------------------------------------------
        /// <summary>
        /// 모든 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RemoveAll()
        {
            foreach (var point in spaceMap.Keys)
            {
                Remove(point);
            }
        }
        
    #endregion

    }
}