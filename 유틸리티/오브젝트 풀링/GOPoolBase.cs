using System;

using UnityEngine;

namespace inonego.Pool
{
    // ============================================================
    /// <summary>
    /// 게임 오브젝트를 생성하는 오브젝트 풀링을 위한 추상 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public abstract class GOPoolBase<T> : PoolBase<T> where T : class
    {
        // ------------------------------------------------------------
        /// <summary>
        /// Acquire되는 경우 오브젝트의 위치를 유지할지에 대한 여부입니다.
        /// </summary>
        // ------------------------------------------------------------
        [HelpBox("Acquire되는 경우 오브젝트의 위치를 유지할지에 대한 여부입니다.")]
        [SerializeField] protected bool worldPositionStays = false;

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 사용할 프리팹입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Header("프리팹")]
        [HelpBox("풀에 사용할 프리팹입니다.")]
        [SerializeField] protected GameObject prefab = null;

        [Header("부모 트랜스폼")]
        [HelpBox("Release되는 경우 게임 오브젝트가 Pool의 하위에 위치합니다.\nAcquire되는 경우 게임 오브젝트가 Active의 하위에 위치합니다.")]
        // ------------------------------------------------------------
        /// <summary>
        /// Release되는 경우 게임 오브젝트가 Pool의 하위에 위치합니다.
        /// </summary>
        // ------------------------------------------------------------
        public Transform Pool => pool;
        [SerializeField] protected Transform pool = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Acquire되는 경우 게임 오브젝트가 Active의 하위에 위치합니다.
        /// </summary>
        // ------------------------------------------------------------
        public Transform Active => active;
        [SerializeField] protected Transform active = null;
    }
}