using System;

using UnityEngine;

namespace inonego.Pool
{
    // ===============================================================================
    /// <summary>
    /// <br/>유니티에서 사용하는 컴포넌트에 대하여 오브젝트 풀링을 할 수 있습니다.
    /// <br/>컴포넌트를 풀링하기 위해서는 해당 T 컴포넌트가 최상단에 포함된 프리팹이 필요합니다.
    /// </summary>
    // ===============================================================================
    [Serializable]
    public class CompPool<T> : GOPoolBase<T> where T : Component
    {
        protected override T Create()
        {
            if (prefab == null)
            {
                throw new NullReferenceException("프리팹이 설정되지 않았습니다.");
            }

            var component = GameObject.Instantiate(prefab, active).GetComponent<T>();

            if (component == null)
            {
                throw new Exception($"프리팹 '{prefab.name}'에서 컴포넌트 '{typeof(T).Name}'를 찾을 수 없습니다.");
            }

            return component;
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