using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    using Serializable;
    
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
                if (despawnable.DespawnFromRegistry != null)
                {
                    despawnable.DespawnFromRegistry.Invoke();
                }
            }
        }
    }
    
    [Serializable]
    public class SpawnedDictionary<TKey, T> : XDictionary_VR<TKey, T>, ISpawnedDictionary<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey> {}

    // ========================================================================
    /// <summary>
    /// 스폰을 관리하기 위한 클래스입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistryBase<TKey, T> : ISpawnRegistry<TKey, T>, IDeepCloneableFrom<SpawnRegistryBase<TKey, T>>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey>
    {

    #region 필드

        [SerializeField, HideInInspector]
        protected SpawnedDictionary<TKey, T> spawned = new();
        public ISpawnedDictionary<TKey, T> Spawned => spawned;

    #endregion

    #region 이벤트

        public event Action<TKey, T> OnSpawn = null;
        public event Action<TKey, T> OnDespawn = null;

    #endregion

    #region 복제 관련

        public void CloneFrom(SpawnRegistryBase<TKey, T> source)
        {
            if (source == null)
            {
                throw new ArgumentNullException("SpawnRegistryBase<TKey, T>.CloneFrom()의 인자가 null입니다.");
            }

            spawned.Clear();

            foreach (var (key, entity) in source.spawned)
            {
                var cloned = entity is IDeepCloneable<T> cloneable ? cloneable.Clone() : entity;
                
                spawned.Add(key, cloned);
            }
        }

    #endregion

    #region 검색 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 해당 키를 가지는 객체를 찾습니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Find(TKey key)
        {
            return spawned.TryGetValue(key, out var value) ? value : null;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 동일 키를 가지는 객체를 찾습니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Find(IKeyable<TKey> keyable)
        {
            if (keyable != null)
            {
                if (keyable.HasKey && spawned.TryGetValue(keyable.Key, out var value))
                {
                    return value;
                }
            }

            return null;
        }

    #endregion

    #region 스폰 / 디스폰 메서드

        protected virtual void OnBeforeSpawn(T spawnable) {}
        protected virtual void OnAfterDespawn(T despawnable) {}

        protected abstract T Acquire();

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 공통 스폰 등록 로직을 수행합니다.
        /// </summary>
        // --------------------------------------------------------------------------------
        protected T SpawnInternal(T spawnable, Action<T> initAction = null)
        {
            // ------------------------------------------------------------
            /// 예외 처리
            // ------------------------------------------------------------
            if (spawnable == null)
            {
                throw new InvalidOperationException("스폰할 객체를 가져올 수 없습니다.");
            }
            
            if (spawnable.IsSpawned)
            {
                throw new InvalidOperationException("이미 스폰된 객체입니다.");
            }

            // ------------------------------------------------------------
            /// 스폰 처리
            // ------------------------------------------------------------
            try
            {
                OnBeforeSpawn(spawnable);
                spawnable.OnBeforeSpawn();

                if (initAction != null)
                {
                    initAction.Invoke(spawnable);
                }

                if (!spawnable.HasKey)
                {
                    throw new InvalidOperationException("스폰할 객체에 키가 설정되어 있지 않습니다.");
                }

                if (spawned.ContainsKey(spawnable.Key))
                {
                    throw new InvalidOperationException($"이미 동일 키({spawnable.Key})가 등록되어 있습니다.");
                }
            }
            catch (Exception)
            {
                // 스폰 중에 예외가 발생하면 객체를 디스폰합니다.
                DespawnInternal(spawnable);

                throw;
            }

            // 디스폰 메서드
            void DoDespawn() => Despawn(spawnable);

            spawnable.IsSpawned = true;
            spawnable.DespawnFromRegistry = DoDespawn;

            spawned.Add(spawnable.Key, spawnable);

            // ------------------------------------------------------------
            /// 스폰 이벤트를 호출합니다.
            // ------------------------------------------------------------
            OnSpawn?.Invoke(spawnable.Key, spawnable);
            
            return spawnable;
        }

        private void Despawn(T despawnable, bool removeFromDictionary = true)
        {
            // ------------------------------------------------------------
            /// 예외 처리
            // ------------------------------------------------------------
            if (despawnable == null)
            {
                throw new ArgumentNullException("디스폰할 객체를 설정해주세요.");
            }

            if (!despawnable.IsSpawned)
            {
                throw new InvalidOperationException("스폰되지 않은 객체를 디스폰할 수 없습니다.");
            }

            if (!despawnable.HasKey)
            {
                throw new InvalidOperationException("디스폰할 객체에 키가 설정되어 있지 않습니다.");
            }

            if (!spawned.ContainsKey(despawnable.Key))
            {
                throw new KeyNotFoundException($"등록되지 않은 키({despawnable.Key})에 해당하는 객체를 디스폰할 수 없습니다.");
            }

            // ------------------------------------------------------------
            /// 디스폰 처리
            // ------------------------------------------------------------
            DespawnInternal(despawnable, removeFromDictionary);

            // ------------------------------------------------------------
            /// 디스폰 이벤트를 호출합니다.
            // ------------------------------------------------------------
            OnDespawn?.Invoke(despawnable.Key, despawnable);
        }

        private void DespawnInternal(T despawnable, bool removeFromDictionary = true)
        {
            despawnable.IsSpawned = false;
            despawnable.DespawnFromRegistry = null;

            if (removeFromDictionary)
            {
                if (despawnable.HasKey)
                {
                    spawned.Remove(despawnable.Key);
                }
            }

            despawnable.OnAfterDespawn();
            OnAfterDespawn(despawnable);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 모든 객체를 디스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void DespawnAll()
        {
            foreach (var (key, entity) in spawned)
            {
                try
                {
                    Despawn(entity, removeFromDictionary: false);
                }
                catch (Exception ex)
                {
                    // 예외가 발생해도 나머지 객체들을 계속 디스폰합니다.
                    #if UNITY_EDITOR
                        Debug.LogException(ex);
                    #else
                        Debug.LogError(ex.Message);
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
    where T : class, ISpawnRegistryObject<TKey>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn()
        {
            var spawnable = Acquire();

            SpawnUsingAquired(spawnable);

            return spawnable;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 만들어진 객체를 이용하여 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SpawnUsingAquired(T spawnable)
        {
            SpawnInternal(spawnable);
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
    public abstract class SpawnRegistry<TKey, T, TParam> : SpawnRegistry<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey>, INeedToInit<TParam>
    {
        protected virtual void OnInit(T spawnable, TParam param) {}

        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn(TParam param)
        {
            var spawnable = Acquire();

            SpawnUsingAquired(spawnable, param);

            return spawnable;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 만들어진 객체를 이용하여 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void SpawnUsingAquired(T spawnable, TParam param)
        {
            void InitAction(T spawnable)
            {
                OnInit(spawnable, param);
                spawnable.Init(param);
            }

            SpawnInternal(spawnable, InitAction);
        }
    }
}