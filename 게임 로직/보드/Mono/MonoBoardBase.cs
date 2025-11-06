using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    /// --------------------------------------------------------------------------------
    /// <summary>
    /// 월드상에 게임 오브젝트로 존재할 수 있는 보드를 표현하기 위한 추상 클래스입니다.
    /// </summary>
    // --------------------------------------------------------------------------------
    [Serializable]
    public abstract class MonoBoardBase<TPoint, TBoardSpace, TPlaceable> : MonoBehaviour, IMonoBoard<TPoint, TBoardSpace, TPlaceable>, IInitNeeded<BoardBase<TPoint, TBoardSpace, TPlaceable>>
    where TPoint : struct
    where TBoardSpace : BoardSpace<TPlaceable>, new()
    where TPlaceable : class, new()
    {

    #region 원본 보드

        [SerializeReference, HideInInspector]
        protected BoardBase<TPoint, TBoardSpace, TPlaceable> board;
        public BoardBase<TPoint, TBoardSpace, TPlaceable> Board => board;

        IBoard<TPoint, TBoardSpace, TPlaceable> IMonoBoard<TPoint, TBoardSpace, TPlaceable>.Board => board;

    #endregion

    #region 필드

        [NonSerialized]
        protected Dictionary<TPoint, GameObject> lTileMap = new();
        public IReadOnlyDictionary<TPoint, GameObject> TileMap => lTileMap;

        [SerializeReference]
        protected IGameObjectProvider lTileProvider = new PrefabGameObjectProvider();
        public IGameObjectProvider TileProvider => lTileProvider;

    #endregion

    #region 초기 설정 및 초기화

        public virtual void Init(BoardBase<TPoint, TBoardSpace, TPlaceable> board)
        {
            if (board == null)
            {
                throw new ArgumentNullException("보드가 null입니다.");
            }
            
            if (this.board != null)
            {
                Release();
            }

            this.board = board;

            InitTileMap(board);

            board.OnAddSpace += OnAddSpace;
            board.OnRemoveSpace += OnRemoveSpace;
        }

        public virtual void Release()
        {
            ReleaseTileMap();

            if (board != null)
            {
                board.OnAddSpace -= OnAddSpace;
                board.OnRemoveSpace -= OnRemoveSpace;
            }

            board = null;
        }

    #endregion

    #region 이벤트 핸들러

        protected virtual void OnAddSpace(TPoint point)
        {
            PlaceTile(point);
        }

        protected virtual void OnRemoveSpace(TPoint point)
        {
            RemoveTile(point);
        }

    #endregion

    #region 메서드

        /// ------------------------------------------------------------
        /// <summary>
        /// 보드상의 좌표를 월드상의 좌표로 변환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public abstract Vector3 ToPos(TPoint point);

        /// ------------------------------------------------------------
        /// <summary>
        /// 지정된 좌표에 타일을 배치할 수 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected virtual bool CanPlaceTile(TPoint point) => true;

        /// ------------------------------------------------------------
        /// <summary>
        /// 지정된 좌표에 타일을 배치합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void PlaceTile(TPoint point)
        {
            bool isPlaced = lTileMap.ContainsKey(point);

            bool canPlace = CanPlaceTile(point);

            // 타일이 배치되어 있지 않고 배치할 수 있는 경우 타일을 배치합니다.
            if (!isPlaced && canPlace)
            {
                var lTile = TileProvider.Acquire();

                lTileMap[point] = lTile;

                lTile.transform.position = ToPos(point);
            }
        }

        /// ------------------------------------------------------------
        /// <summary>
        /// 지정된 좌표의 타일을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RemoveTile(TPoint point)
        {
            RemoveTile(point, removeFromMap: true);
        }

        protected virtual void RemoveTile(TPoint point, bool removeFromMap = true)
        {
            if (lTileMap.TryGetValue(point, out var lTile))
            {
                TileProvider.Release(lTile);

                if (removeFromMap)
                {
                    lTileMap.Remove(point);
                }
            }
        }

        /// ------------------------------------------------------------
        /// <summary>
        /// 모든 타일을 제거합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void RemoveTileAll()
        {
            foreach (var point in lTileMap.Keys)
            {
                RemoveTile(point, removeFromMap: false);
            }

            lTileMap.Clear();
        }

    #endregion

    #region 새로고침

        public void ReBuildTileMap()
        {
            if (board == null)
            {
                throw new InvalidOperationException("보드가 초기화되지 않았습니다. Init()을 먼저 호출해주세요.");
            }

            ReleaseTileMap();
            InitTileMap(board);
        }

        protected void InitTileMap(BoardBase<TPoint, TBoardSpace, TPlaceable> board)
        {
            if (board == null)
            {
                throw new ArgumentNullException("보드가 null입니다.");
            }

            foreach (var (point, space) in board)
            {
                PlaceTile(point);
            }
        }

        protected void ReleaseTileMap()
        {
            RemoveTileAll();
        }

    #endregion

    }
}