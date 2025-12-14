using System;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 정수 인덱스 기반 보드 공간입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class BoardSpace<TPlaceable> : BoardSpaceBase<int, TPlaceable>
    where TPlaceable : class
    {
        // NONE
    }
}

