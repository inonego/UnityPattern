using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    
    // ============================================================
    /// <summary>
    /// 3D 크기(Width, Height, Depth) 범위 내에서 동작하는 보드입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Board3D<TPlaceable> : Board3D<BoardSpace<TPlaceable>, TPlaceable>
    where TPlaceable : class, new()
    {
        public Board3D(int width, int height, int depth, bool init = true) : base(width, height, depth, init) {}
    }

    // ============================================================
    /// <summary>
    /// 3D 크기(Width, Height, Depth) 범위 내에서 동작하는 보드입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class Board3D<TBoardSpace, TPlaceable> : BoardBase<Vector3Int, TBoardSpace, TPlaceable>
    where TBoardSpace : BoardSpace<TPlaceable>, new()
    where TPlaceable : class, new()
    {
        [SerializeField]
        protected int width;

        [SerializeField]
        protected int height;

        [SerializeField]
        protected int depth;

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
        /// 보드의 깊이 크기입니다.
        /// </summary>
        // ------------------------------------------------------------
        public int Depth => depth;

        // ------------------------------------------------------------
        /// <summary>
        /// 보드의 크기입니다.
        /// </summary>
        // ------------------------------------------------------------
        public Vector3Int Size => new Vector3Int(width, height, depth);

        // ------------------------------------------------------------
        /// <summary>
        /// 포인트가 유효한 보드 범위 내에 있는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override bool IsValidPoint(Vector3Int point)
        {
            return 0 <= point.x && point.x < width && 0 <= point.y && point.y < height && 0 <= point.z && point.z < depth;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 생성자에서 크기를 지정하고 공간을 초기화합니다.
        /// </summary>
        // ------------------------------------------------------------
        public Board3D(int width, int height, int depth, bool init = true)
        {
            this.width = Math.Max(0, width);
            this.height = Math.Max(0, height);
            this.depth = Math.Max(0, depth);

            if (init)
            {
                Init();
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 보드의 모든 유효 좌표에 대해 공간을 초기화합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected void Init()
        {
            spaceMap.Clear();

            for (int z = 0; z < depth; z++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < width; x++)
                    {
                        var p = new Vector3Int(x, y, z);

                        AddSpace(p);
                    }
                }
            }
        }
    }
}



