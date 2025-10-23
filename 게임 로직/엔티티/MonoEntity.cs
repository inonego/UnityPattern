using UnityEngine;

using System;

namespace inonego
{

    public interface IMonoEntity : IKeyable<ulong>
    {
        public Entity Entity { get; }
    }

    public interface IMonoEntity<TEntity> : IKeyable<ulong>
    where TEntity : Entity
    {
        public TEntity Entity { get; }
    }

    [Serializable]
    public abstract class MonoEntity<TEntity> : MonoBehaviour, IMonoEntity<TEntity>, IMonoEntity, ISpawnable<TEntity>, IDespawnable
    where TEntity : Entity
    {
        #region 키 설정

            public ulong Key
            {
                get
                {
                    if (entity != null && entity.HasKey)
                    {
                        return entity.Key;
                    }

                    throw new InvalidOperationException("키가 설정되어 있지 않습니다.");
                }
            }

            public bool HasKey => entity != null && entity.HasKey;

        #endregion

        #region 필드

            [SerializeField, ReadOnly]
            protected bool isSpawned = false;
            public bool IsSpawned => isSpawned;

            [SerializeReference]
            protected TEntity entity = null;
            public TEntity Entity => entity;

            Entity IMonoEntity.Entity => entity;
            
        #endregion

        #region 인터페이스 구현
            
            bool ISpawnedFlag.IsSpawned { get => isSpawned; set => isSpawned = value; }

            Action IDespawnable.DespawnFromRegistry { get; set; }

            void ISpawnable<TEntity>.OnBeforeSpawn(TEntity entity) => OnBeforeSpawn(entity);
            void IDespawnable.OnAfterDespawn() => OnAfterDespawn();

            protected virtual void OnBeforeSpawn(TEntity entity)
            {
                if (entity == null)
                {
                    throw new ArgumentNullException("엔티티가 null입니다.");
                }

                this.entity = entity;
            }

            protected virtual void OnAfterDespawn()
            {
                entity = null;
            }

        #endregion
    }
}