using System;

using UnityEngine;

namespace inonego
{
    // ===============================================================================
    /// <summary>
    /// <br/>유니티에서 사용하는 컴포넌트에 사용 가능한 풀입니다.
    /// <br/>컴포넌트를 풀링하기 위해서는 해당 T 컴포넌트가 최상단에 포함된 프리팹이 필요합니다.
    /// </summary>
    // ===============================================================================
    [Serializable]
    public class ComponentPool<T> : Pool<T> where T : Component
    {
        // ------------------------------------------------------------
        /// <summary>
        /// Acquire되는 경우 오브젝트의 위치를 유지할지 여부입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] private bool worldPositionStays = false;

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 사용할 프리팹입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Header("프리팹")]
        [SerializeField] private GameObject prefab = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Release되는 경우 오브젝트가 위치할 부모 트랜스폼입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Header("부모 트랜스폼")]
        [SerializeField] private Transform pool = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Acquire되는 경우 오브젝트가 위치할 부모 트랜스폼입니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField] private Transform active = null;
        
        protected override T Create()
        {
            return GameObject.Instantiate(prefab, active).GetComponent<T>();
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