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
    public abstract class EntitySpawnRegistryBase<TEntity> : SpawnRegistryBase<ulong, TEntity> 
    where TEntity : Entity
    {

    #region 필드

        [SerializeReference]
        private IKeyGenerator<ulong> keyGenerator = new IncreKeyGenerator();
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

    }

    [Serializable]
    public abstract class EntitySpawnRegistry<TEntity> : EntitySpawnRegistryBase<TEntity>
    where TEntity : Entity
    {
        public TEntity Spawn()
        {
            return SpawnInternal();
        }
    }

    [Serializable]
    public abstract class EntitySpawnRegistry<TEntity, TParam> : EntitySpawnRegistryBase<TEntity>
    where TEntity : Entity, IInitNeeded<TParam>
    {
        protected virtual void OnInit(TEntity spawnable, TParam param) {}
        
        public TEntity Spawn(TParam param)
        {
            void InitAction(TEntity spawnable)
            {
                OnInit(spawnable, param);
                spawnable.Init(param);
            }

            return SpawnInternal(InitAction);
        }
    }

}