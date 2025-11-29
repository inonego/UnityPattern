using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    using Serializable;

    // ============================================================
    /// <summary>
    /// 보드를 표현하기 위한 추상 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public abstract class BoardBase<TVector, TIndex, TSpace, TPlaceable> : IBoard<TVector, TIndex, TPlaceable>, IEnumerable<KeyValuePair<TVector, TSpace>>, IEnumerable
    where TVector : struct where TIndex : struct
    where TSpace : BoardSpaceBase<TIndex, TPlaceable>, new()
    where TPlaceable : class, new()
    {

    #region 내부 구조체

        [Serializable]
        public struct Point : IBoardPoint<TVector, TIndex>, IEquatable<Point>
        {
            public TVector Vector;
            public TIndex Index;

            TVector IBoardPoint<TVector, TIndex>.Vector => Vector;
            TIndex IBoardPoint<TVector, TIndex>.Index => Index;

            public Point(TVector vector, TIndex index) => (Vector, Index) = (vector, index);

            public static implicit operator TVector(Point point) => point.Vector;
            public static implicit operator TIndex(Point point) => point.Index;

            public static implicit operator (TVector Vector, TIndex Index)(Point point) => (point.Vector, point.Index);
            public static implicit operator Point((TVector Vector, TIndex Index) point) => new Point(point.Vector, point.Index);

            public static bool operator ==(Point left, Point right) => left.Equals(right);
            public static bool operator !=(Point left, Point right) => !left.Equals(right);

        #region 기본 메서드

            public bool Equals(Point other) => Equals(Vector, other.Vector) && Equals(Index, other.Index);
            public override bool Equals(object obj) => obj is Point other && Equals(other);
            public override int GetHashCode() => HashCode.Combine(Vector, Index);
            public override string ToString() => $"({Vector} / {Index})";
            
        #endregion

        }
    
    #endregion

    #region 필드

        [SerializeField]
        protected XDictionary_VV<TVector, TSpace> spaceMap = new();

        [SerializeField]
        protected XDictionary_RV<TPlaceable, Point> pointMap = new();

        protected virtual bool IsValidVector(TVector vector) => true;

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 벡터에 있는 공간을 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TSpace this[TVector vector]
        {
            get
            {
                var isValidVector = IsValidVector(vector);

                if (isValidVector)
                {
                    return spaceMap.TryGetValue(vector, out var space) ? space : null;
                }

                return null;
            }
        }

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 벡터와 인덱스에 있는 객체를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPlaceable this[TVector vector, TIndex index]
        {
            get
            {
                TSpace space = this[vector];

                if (space != null) 
                {
                    return space[index];
                }

                return null;
            }
        }

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 포인트에 있는 객체를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPlaceable this[IBoardPoint<TVector, TIndex> point]
        {
            get
            {
                if (point != null) 
                {
                    return this[point.Vector, point.Index];
                }

                return null;
            }
        }

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 포인트에 있는 객체를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public TPlaceable this[Point point] => this[point.Vector, point.Index];

        //------------------------------------------------------------
        /// <summary>
        /// 지정된 객체의 위치를 반환합니다.
        /// </summary>
        //------------------------------------------------------------
        public Point? this[TPlaceable placeable]
        {
            get => pointMap.TryGetValue(placeable, out var point) ? point : null;
        }

        IBoardPoint<TVector, TIndex> IBoard<TVector, TIndex, TPlaceable>.this[TPlaceable placeable] => this[placeable];

    #endregion

    #region 이벤트

        public event Action<TVector, TIndex, TPlaceable> OnPlace = null;
        public event Action<TVector, TIndex, TPlaceable> OnRemove = null;

        public event Action<TVector> OnAddSpace = null;
        public event Action<TVector> OnRemoveSpace = null;

    #endregion

    #region 인터페이스 구현

        IBoardSpace<TIndex, TPlaceable> IBoard<TVector, TIndex, TPlaceable>.this[TVector vector] => this[vector];

        IEnumerator<KeyValuePair<TVector, TSpace>> IEnumerable<KeyValuePair<TVector, TSpace>>.GetEnumerator()
        {
            foreach (var (vector, space) in spaceMap)
            {
                yield return new KeyValuePair<TVector, TSpace>(vector, space);
            }
        }

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
        public virtual TSpace CreateSpace() => new TSpace();

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치에 공간을 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void AddSpace(TVector vector, bool invokeEvent = true)
        {
            if (!IsValidVector(vector)) return;

            // 이미 공간이 존재하면 중복 생성하지 않습니다.
            if (spaceMap.ContainsKey(vector))
            {
                throw new InvalidOperationException("이미 해당 위치에 공간이 존재합니다. RemoveSpace() 메서드를 사용하여 제거 후 다시 추가해주세요.");
            }

            spaceMap[vector] = CreateSpace();

            if (invokeEvent)
            {
                OnAddSpace?.Invoke(vector);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치의 공간을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RemoveSpace(TVector vector, bool invokeEvent = true)
        {
            RemoveSpace(vector, invokeEvent, removeFromMap: true);
        }

        protected virtual void RemoveSpace(TVector vector, bool invokeEvent = true, bool removeFromMap = true)
        {
            if (!IsValidVector(vector)) return;

            if (!spaceMap.TryGetValue(vector, out var space)) return;

            // 공간에 있는 모든 객체를 제거합니다.
            Remove(vector, invokeEvent);

            if (removeFromMap)
            {
                // 공간을 제거합니다.
                spaceMap.Remove(vector);
            }

            if (invokeEvent)
            {
                OnRemoveSpace?.Invoke(vector);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 모든 공간을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void RemoveSpaceAll(bool invokeEvent = true)
        {
            foreach (var (vector, _) in spaceMap)
            {
                RemoveSpace(vector, invokeEvent, removeFromMap: false);
            }

            spaceMap.Clear();
        }

    #endregion

    #region 배치 및 제거 메서드

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치에 객체를 배치할 수 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool CanPlace(TVector vector, TIndex index, TPlaceable placeable = null)
        {
            TSpace space = this[vector];

            // 공간이 존재하고 배치 가능한지 확인합니다.
            return space != null && space.CanPlace(index, placeable);
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치에 객체를 배치합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Place(TVector vector, TIndex index, TPlaceable placeable, bool invokeEvent = true)
        {    
            if (placeable == null) throw new ArgumentNullException(nameof(placeable), "배치할 객체를 설정해주세요.");

            TSpace space = this[vector];
            
            if (space == null) 
            {
                throw new InvalidOperationException("해당 위치에 공간이 존재하지 않습니다. AddSpace() 메서드를 사용하여 추가해주세요.");
            }

            if (!CanPlace(vector, index, placeable))
            {
                throw new InvalidOperationException("해당 위치에 객체를 배치할 수 없습니다. CanPlace() 메서드를 사용하여 배치 가능한지 확인해주세요.");
            }

            // 원래 위치에서 객체를 제거합니다.
            Remove(placeable, invokeEvent);

            // 공간에 객체를 배치합니다.
            space.Place(index, placeable);
            
            // 객체의 위치를 업데이트합니다.
            pointMap[placeable] = new Point(vector, index);

            if (invokeEvent)
            {
                OnPlace?.Invoke(vector, index, placeable);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 위치의 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Remove(TVector vector, TIndex index, bool invokeEvent = true)
        {
            Remove(vector, index, invokeEvent, removeFromSpace: true);
        }

        protected virtual void Remove(TVector vector, TIndex index, bool invokeEvent = true, bool removeFromSpace = true)
        {
            TSpace space = this[vector];
            if (space == null) return;

            var placeable = space[index];
            if (placeable == null) return;

            if (removeFromSpace)
            {
                // 공간에서 객체를 제거합니다.
                space.Remove(index);
            }
            
            // 객체의 위치를 제거합니다.
            pointMap.Remove(placeable);

            if (invokeEvent)
            {
                OnRemove?.Invoke(vector, index, placeable);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Remove(TPlaceable placeable, bool invokeEvent = true)
        {
            if (placeable == null)
            {
                throw new ArgumentNullException("제거할 객체를 설정해주세요.");
            }

            if (pointMap.TryGetValue(placeable, out var point))
            {
                Remove(point.Vector, point.Index, invokeEvent);
            }
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 벡터의 모든 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Remove(TVector vector, bool invokeEvent = true)
        {
            TSpace space = this[vector];
            if (space == null) return;

            // 해당 공간의 모든 객체를 제거합니다.
            foreach (var (index, _) in space)
            {
                Remove(vector, index, invokeEvent, removeFromSpace: false);
            }

            // 공간의 모든 객체를 한 번에 제거합니다.
            space.RemoveAll();
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 모든 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RemoveAll(bool invokeEvent = true)
        {
            foreach (var (vector, _) in spaceMap)
            {
                Remove(vector, invokeEvent);
            }
        }
        
    #endregion

    #region IBoardPoint를 통한 배치 및 제거 메서드

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 포인트에 객체를 배치할 수 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool CanPlace(IBoardPoint<TVector, TIndex> point, TPlaceable placeable = null)
        {
            if (point == null)
            {
                throw new ArgumentNullException("포인트를 설정해주세요.");
            }

            return CanPlace(point.Vector, point.Index, placeable);
        }

        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 포인트에 객체를 배치합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Place(IBoardPoint<TVector, TIndex> point, TPlaceable placeable, bool invokeEvent = true)
        {
            if (point == null) 
            {
                throw new ArgumentNullException("포인트를 설정해주세요.");
            }

            Place(point.Vector, point.Index, placeable, invokeEvent);
        }
        
        // ---------------------------------------------------------------
        /// <summary>
        /// 지정된 포인트의 객체를 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Remove(IBoardPoint<TVector, TIndex> point, bool invokeEvent = true)
        {
            if (point == null)
            {
                throw new ArgumentNullException("포인트를 설정해주세요.");
            }

            Remove(point.Vector, point.Index, invokeEvent);
        }

    #endregion

    }
}