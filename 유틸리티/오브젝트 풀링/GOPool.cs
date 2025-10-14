using System;

using UnityEngine;

namespace inonego.Pool
{
    // ===============================================================================
    /// <summary>
    /// 유니티에서 사용하는 게임 오브젝트에 대하여 오브젝트 풀링을 할 수 있습니다.
    /// </summary>
    // ===============================================================================
    [Serializable]
    public class GOPool : GOPoolBase<GameObject>
    {
        protected override GameObject Create()
        {
            if (prefab == null)
            {
                throw new NullReferenceException("프리팹이 설정되지 않았습니다.");
            }

            return GameObject.Instantiate(prefab, active);
        }

        protected override void OnAcquire(GameObject gameObject)
        {
            gameObject.transform.SetParent(active, worldPositionStays);
            gameObject.SetActive(true);
        }

        protected override void OnRelease(GameObject gameObject)
        {
            gameObject.transform.SetParent(pool, worldPositionStays);
            gameObject.SetActive(false);
        }
    }
}