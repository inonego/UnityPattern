using System;

using UnityEngine;

namespace inonego
{
    // ======================================================================== 
    /// <summary>
    /// 모노 엔티티 스폰 레지스트리를 관리하기 위한 클래스입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public abstract class MonoEntitySpawnRegistry<TMonoEntity, TEntity> : SpawnRegistryBase<ulong, TMonoEntity>
    where TMonoEntity : MonoEntity<TEntity>
    where TEntity : Entity
    {

    #region 필드

        [SerializeReference]
        private IGameObjectProvider gameObjectProvider = new PrefabGameObjectProvider();
        public IGameObjectProvider GameObjectProvider => gameObjectProvider;

    #endregion

    #region 생성자

        public MonoEntitySpawnRegistry() : base() {}

        public MonoEntitySpawnRegistry(IGameObjectProvider gameObjectProvider) : this()
        {
            if (gameObjectProvider == null)
            {
                throw new ArgumentNullException("GameObjectProvider가 null입니다.");
            }
            
            this.gameObjectProvider = gameObjectProvider;
        }

    #endregion

    #region 메서드

        protected override TMonoEntity Acquire()
        {
            var gameObject = GameObjectProvider.Acquire();

            var monoEntity = gameObject.GetComponent<TMonoEntity>();

            if (monoEntity == null)
            {
                throw new InvalidOperationException($"모노 엔티티 컴포넌트가 {gameObject.name}에 없습니다.");
            }

            return monoEntity;
        }

        protected virtual void OnInit(TMonoEntity spawnable, TEntity entity) {}

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 모노 엔티티를 스폰합니다.
        /// </summary>
        // --------------------------------------------------------------------------------
        public TMonoEntity Spawn(TEntity entity)
        {
            void InitAction(TMonoEntity spawnable)
            {
                OnInit(spawnable, entity);
                spawnable.Init(entity);
            }

            return SpawnInternal(InitAction);
        }

    #endregion

    #region 스폰 레지스트리 연결 메서드

        [SerializeReference]
        private EntitySpawnRegistry<TEntity> linkedRegistry = null;
        public EntitySpawnRegistry<TEntity> LinkedRegistry => linkedRegistry;

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 엔티티 스폰 레지스트리와 연동하여 엔티티의 상태를 동기화합니다.
        /// </summary>
        // --------------------------------------------------------------------------------
        public void LinkTo(EntitySpawnRegistry<TEntity> registry, bool reSpawnAll = false)
        {
            RemoveLink();
            
            if (registry == null)
            {
                throw new ArgumentNullException("연결할 엔티티 스폰 레지스트리가 null입니다.");
            }
            
            registry.OnSpawn += OnEntitySpawn;
            registry.OnDespawn += OnEntityDespawn;

            linkedRegistry = registry;

            if (reSpawnAll)
            {
                ReSpawnAll();
            }
        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 엔티티 스폰 레지스트리와의 연동을 해제합니다.
        /// </summary>
        // --------------------------------------------------------------------------------
        public void RemoveLink()
        {
            if (linkedRegistry != null)
            {
                linkedRegistry.OnSpawn -= OnEntitySpawn;
                linkedRegistry.OnDespawn -= OnEntityDespawn;

                linkedRegistry = null;
            }
        }

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 엔티티 스폰 레지스트리와 다시 동기화하여 모든 객체를 디스폰하고 다시 스폰하도록 합니다.
        /// </summary>
        // --------------------------------------------------------------------------------
        public void ReSpawnAll()
        {
            if (linkedRegistry == null)
            {
                throw new InvalidOperationException("엔티티 스폰 레지스트리와 연결되어 있지 않습니다.");
            }

            DespawnAll();
            
            foreach (var (key, entity) in linkedRegistry.Spawned)
            {
                Spawn(entity);
            }
        }
    
    #endregion

    #region 이벤트 핸들러

        protected virtual void OnEntitySpawn(ulong key, TEntity entity)  
        {
            Spawn(entity);
        }

        protected virtual void OnEntityDespawn(ulong key, TEntity entity)
        {
            if (Spawned.TryGetValue(key, out var spawned))
            {
                var monoEntity = spawned.Value;
                
                if (monoEntity != null)
                {
                    monoEntity.Despawn();
                }
            }
        }

    #endregion

    }
}