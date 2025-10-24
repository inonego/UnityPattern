using System;

using UnityEngine;

namespace inonego
{
    public abstract class MonoEntityBehaviour : MonoBehaviour
    {
        [Header("엔티티", order = -999)]
        [SerializeReference, ReadOnly]
        private IMonoEntity monoEntity = null;
        public IMonoEntity MonoEntity => monoEntity;

        public Entity Entity
        {
            get
            {
                if (monoEntity == null)
                {
                    throw new InvalidOperationException("모노 엔티티가 null입니다.");
                }

                return monoEntity.Entity;
            }
        }

        private void Awake()
        {
            GetComponents();
        }

        private void GetComponents()
        {
            if (monoEntity == null)
            {
                monoEntity = GetComponentInParent<IMonoEntity>();
            }

            if (monoEntity == null)
            {
                throw new NullReferenceException("MonoEntityBehaviour의 부모 오브젝트에 MonoEntity 컴포넌트가 없습니다.");
            }
        }
    }
}