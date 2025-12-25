using System;

using UnityEngine;

namespace inonego
{
    // ======================================================================== 
    /// <summary>
    /// 엔티티 스폰 레지스트리를 관리하기 위한 클래스입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public abstract class EntitySpawnRegistryBase<TEntity> : SpawnRegistryBase<ulong, TEntity>, IDeepCloneableFrom<EntitySpawnRegistryBase<TEntity>>
    where TEntity : Entity
    {

    #region 필드

        [SerializeReference]
        protected IKeyGenerator<ulong> keyGenerator = new IncreKeyGenerator();
        public IKeyGenerator<ulong> KeyGenerator => keyGenerator;

    #endregion

    #region 생성자

        public EntitySpawnRegistryBase() : base() {}

        public EntitySpawnRegistryBase(IKeyGenerator<ulong> keyGenerator) : this()
        {
            if (keyGenerator == null)
            {
                throw new ArgumentNullException("KeyGenerator가 null입니다.");
            }

            this.keyGenerator = keyGenerator;
        }

    #endregion

    #region 메서드

        protected override void OnBeforeSpawn(TEntity spawnable)
        {
            // 스폰될때 키를 생성하여 엔티티에 설정합니다.
            var key = keyGenerator.Generate();

            spawnable.SetKey(key);

            base.OnBeforeSpawn(spawnable);
        }

        protected override void OnAfterDespawn(TEntity despawnable)
        {
            base.OnAfterDespawn(despawnable);

            despawnable.ClearKey();
        }

    #endregion

    #region 복제 관련

        public void CloneFrom(EntitySpawnRegistryBase<TEntity> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("EntitySpawnRegistryBase<TEntity>.CloneFrom()의 인자가 null입니다.");
            }

            keyGenerator = source.keyGenerator.Clone();

            base.CloneFrom(source);
        }

    #endregion
    }

    [Serializable]
    public abstract class EntitySpawnRegistry<TEntity> : EntitySpawnRegistryBase<TEntity>
    where TEntity : Entity
    {
        protected abstract TEntity Acquire();

        // ------------------------------------------------------------
        /// <summary>
        /// 엔티티를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected TEntity Spawn()
        {
            var entity = Acquire();

            Spawn(entity);

            return entity;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 엔티티를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual bool TrySpawn(out TEntity spawned)
        {
            spawned = Spawn();

            return spawned != null;
        }
    }

    [Serializable]
    public abstract class EntitySpawnRegistry<TEntity, TParam> : EntitySpawnRegistryBase<TEntity>
    where TEntity : Entity, INeedToInit<TParam>
    {
        protected virtual void OnInit(TEntity spawnable, TParam param) {}

        protected abstract TEntity Acquire(TParam param);

        // ------------------------------------------------------------
        /// <summary>
        /// 엔티티를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected TEntity Spawn(TParam param)
        {
            var entity = Acquire(param);

            void InitAction(TEntity spawnable)
            {
                OnInit(spawnable, param);
                spawnable.Init(param);
            }

            Spawn(entity, InitAction);

            return entity;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 엔티티를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual bool TrySpawn(TParam param, out TEntity spawned)
        {
            spawned = Spawn(param);

            return spawned != null;
        }
    }

}