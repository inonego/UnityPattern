using System;

using UnityEngine;

namespace inonego
{
    public abstract class MonoEntityBehaviour : MonoBehaviour
    {
        [Header("엔티티", order = -999)]
        [SerializeField, ReadOnly]
        private MonoEntity entity = null;
        public MonoEntity Entity => entity;

        private void Awake()
        {
            GetComponents();
        }

        private void GetComponents()
        {
            if (entity == null)
            {
                entity = GetComponentInParent<MonoEntity>();
            }

            if (entity == null)
            {
                throw new Exception("HitBox의 부모 오브젝트에 Entity 컴포넌트가 없습니다.");
            }
        }
    }
}