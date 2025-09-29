using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 2D 크기(Width, Height) 범위 내에서 동작하는 보드입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Board2D<TBoardSpace, TPlaceable> : BoardBase<Vector2Int, TBoardSpace, TPlaceable>
    where TBoardSpace : BoardSpace<TPlaceable>, new()
    where TPlaceable : class, new()
    {
        [SerializeField]
        private int width;

        [SerializeField]
        private int height;

        // ------------------------------------------------------------
        /// <summary>
        /// 보드의 가로 크기입니다.
        /// </summary>
        // ------------------------------------------------------------
        public int Width => width;

        // ------------------------------------------------------------
        /// <summary>
        /// 보드의 세로 크기입니다.
        /// </summary>
        // ------------------------------------------------------------
        public int Height => height;

        // ------------------------------------------------------------
        /// <summary>
        /// 보드의 크기입니다.
        /// </summary>
        // ------------------------------------------------------------
        public Vector2Int Size => new Vector2Int(width, height);

        // ------------------------------------------------------------
        /// <summary>
        /// 포인트가 유효한 보드 범위 내에 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override bool IsValidPoint(Vector2Int point)
        {
            return 0 <= point.x && point.x < width && 0 <= point.y && point.y < height;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 생성자에서 크기를 지정하고 공간을 초기화합니다.
        /// </summary>
        // ------------------------------------------------------------
        public Board2D(int width, int height)
        {
            this.width = Math.Max(0, width);
            this.height = Math.Max(0, height);

            Initialize();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 보드의 모든 유효 좌표에 대해 공간을 초기화합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void Initialize()
        {
            spaceMap.Clear();

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    var p = new Vector2Int(x, y);
                    
                    AddSpace(p);
                }
            }
        }
    }
}


