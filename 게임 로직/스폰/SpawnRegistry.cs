using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    using Serializable;

    [Serializable]
    public class SpawnedDictionary<TKey, T> : XDictionary<TKey, SerializeReferenceWrapper<T>>, ISpawnedDictionary<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnedFlag, IKeyable<TKey> {}

    public interface ISpawnedDictionary<TKey, T> : IReadOnlyDictionary<TKey, SerializeReferenceWrapper<T>>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnedFlag, IKeyable<TKey> {}
    
    public static class SpawnRegistryUtility
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 디스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void Despawn(this IDespawnable despawnable)
        {
            if (despawnable != null)
            {
                if (!despawnable.IsSpawned)
                {
                    throw new InvalidOperationException("스폰되지 않은 객체를 디스폰할 수 없습니다.");
                }

                if (despawnable.DespawnFromRegistry != null)
                {
                    despawnable.DespawnFromRegistry.Invoke();
                }
            }
        }
    }

    // ========================================================================
    /// <summary>
    /// 스폰을 관리하기 위한 클래스입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistryBase<TKey, T> 
    where TKey : IEquatable<TKey>
    where T : class, IDespawnable, IKeyable<TKey>
    {

    #region 필드

        [SerializeField]
        private InvokeEventFlag invokeEvent = new();
        public InvokeEventFlag InvokeEvent => invokeEvent;

        [SerializeField]
        private SpawnedDictionary<TKey, T> spawned = new();
        public ISpawnedDictionary<TKey, T> Spawned => spawned;

        [SerializeReference]
        private IKeyGenerator<TKey> keyGenerator = null;
        public IKeyGenerator<TKey> KeyGenerator => keyGenerator;

    #endregion

    #region 이벤트

        public event Action<SpawnRegistryBase<TKey, T>, T> OnSpawn = null;
        public event Action<SpawnRegistryBase<TKey, T>, T> OnDespawn = null;

    #endregion

    #region 생성자

        protected SpawnRegistryBase() {}

        public SpawnRegistryBase(IKeyGenerator<TKey> keyGenerator)
        {
            if (keyGenerator == null)
            {
                throw new ArgumentNullException("키 생성기를 설정해주세요.");
            }

            this.keyGenerator = keyGenerator;
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 공통 스폰 등록 로직을 수행합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected T RegisterAndSpawn(T spawnable, Action<T> invokeSpawnable)
        {
            if (spawnable == null)
            {
                throw new ArgumentNullException("스폰할 객체를 설정해주세요.");
            }
            
            if (spawnable.IsSpawned)
            {
                throw new InvalidOperationException("이미 스폰된 객체입니다.");
            }

            spawnable.Key = KeyGenerator.Generate();

            if (spawned.ContainsKey(spawnable.Key))
            {
                throw new InvalidOperationException($"이미 동일 키({spawnable.Key})가 등록되어 있습니다.");
            }

            // 디스폰 메서드
            void DoDespawn() => Despawn(spawnable);

            spawnable.IsSpawned = true;
            spawnable.DespawnFromRegistry = DoDespawn;

            spawned.Add(spawnable.Key, spawnable);

            try
            {
                invokeSpawnable?.Invoke(spawnable);
            }
            catch (Exception)
            {
                // 스폰 중에 예외가 발생하면 객체를 디스폰합니다.
                DespawnInternal(spawnable);

                throw;
            }

            if (invokeEvent.Value)
            {
                OnSpawn?.Invoke(this, spawnable);
            }

            return spawnable;
        }

        private void Despawn(T despawnable, bool removeFromDictionary = true)
        {
            if (despawnable == null)
            {
                throw new ArgumentNullException("디스폰할 객체를 설정해주세요.");
            }

            if (!despawnable.IsSpawned)
            {
                throw new InvalidOperationException("스폰되지 않은 객체를 디스폰할 수 없습니다.");
            }

            if (!spawned.ContainsKey(despawnable.Key))
            {
                throw new KeyNotFoundException($"등록되지 않은 키({despawnable.Key})에 해당하는 객체를 디스폰할 수 없습니다.");
            }

            DespawnInternal(despawnable, removeFromDictionary);

            if (invokeEvent.Value)
            {
                OnDespawn?.Invoke(this, despawnable);
            }
        }

        private void DespawnInternal(T despawnable, bool removeFromDictionary = true)
        {
            despawnable.IsSpawned = false;
            despawnable.DespawnFromRegistry = null;

            if (removeFromDictionary)
            {
                spawned.Remove(despawnable.Key);
            }
            
            despawnable.OnDespawn();

            despawnable.Key = default;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 객체를 디스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void DespawnAll()
        {
            foreach (var entity in spawned.Values)
            {
                try
                {
                    Despawn(entity, removeFromDictionary: false);
                }
                catch (Exception ex)
                {

                #if UNITY_EDITOR
                    // 예외가 발생해도 나머지 객체들을 계속 디스폰합니다.
                    Debug.LogException(ex);
                #endif

                }
            }

            // 스폰된 객체 목록을 초기화합니다.
            spawned.Clear();
        }

    #endregion

    }
    
    // ========================================================================
    /// <summary>
    /// 스폰을 관리하기 위한 클래스입니다.
    /// </summary>
    /// <typeparam name="TKey">스폰된 객체의 키의 타입입니다.</typeparam>
    /// <typeparam name="T">스폰 가능한 객체의 타입입니다.</typeparam>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistry<TKey, T> : SpawnRegistryBase<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnable, IDespawnable, IKeyable<TKey>
    {
        public SpawnRegistry(IKeyGenerator<TKey> keyGenerator) : base(keyGenerator) {}

        // ------------------------------------------------------------
        /// <summary>
        /// 스폰할 객체를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T Acquire();

        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn()
        {
            var spawnable = Acquire();

            if (spawnable == null)
            {
                throw new InvalidOperationException("스폰할 객체를 가져올 수 없습니다.");
            }

            void OnSpawn(T spawnable) => spawnable.OnSpawn();

            return RegisterAndSpawn(spawnable, OnSpawn);
        }
    }

    // ========================================================================
    /// <summary>
    /// 스폰을 관리하기 위한 클래스입니다.
    /// </summary>
    /// <typeparam name="TKey">스폰된 객체의 키의 타입입니다.</typeparam>
    /// <typeparam name="T">스폰 가능한 객체의 타입입니다.</typeparam>
    /// <typeparam name="TParam">스폰 시 전달할 매개변수의 타입입니다.</typeparam>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistry<TKey, T, TParam> : SpawnRegistryBase<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnable<TParam>, IDespawnable, IKeyable<TKey>
    {
        public SpawnRegistry(IKeyGenerator<TKey> keyGenerator) : base(keyGenerator) {}

        // ------------------------------------------------------------
        /// <summary>
        /// 스폰할 객체를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T Acquire(TParam param);

        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn(TParam param)
        {
            var spawnable = Acquire(param);

            if (spawnable == null)
            {
                throw new InvalidOperationException("스폰할 객체를 가져올 수 없습니다.");
            }

            void OnSpawn(T spawnable) => spawnable.OnSpawn(param);

            return RegisterAndSpawn(spawnable, OnSpawn);
        }
    }
}