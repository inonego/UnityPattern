using System;

using UnityEngine;

namespace inonego
{
    // ============================================================
    /// <summary>
    /// 유니티에서 사용하는 컴포넌트에 사용 가능한 풀입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class ComponentPool<T> : Pool<T> where T : Component
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 사용할 프리팹입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] private GameObject prefab = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Release되는 경우 오브젝트가 위치할 부모 트랜스폼입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] private Transform pool = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Acquire되는 경우 오브젝트가 위치할 부모 트랜스폼입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] private Transform active = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Acquire되는 경우 오브젝트의 위치를 유지할지 여부입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] private bool worldPositionStays = false;
        
        protected override T Create()
        {
            return GameObject.Instantiate(prefab, active).GetComponentInChildren<T>();
        }

        protected override void OnAcquire(T item)
        {
            item.transform.SetParent(active, worldPositionStays);
            item.gameObject.SetActive(true);
        }

        protected override void OnRelease(T item)
        {
            item.transform.SetParent(pool, worldPositionStays);
            item.gameObject.SetActive(false);
        }
    }
}