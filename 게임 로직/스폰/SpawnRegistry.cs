using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    using Serializable;

    [Serializable]
    public class SpawnedDictionary<T> : XDictionary<string, SerializeReferenceWrapper<T>>, ISpawnedDictionary<T> where T : class, ICanSpawnFromRegistry {}
    public interface ISpawnedDictionary<T> : IReadOnlyDictionary<string, SerializeReferenceWrapper<T>> where T : class, ICanSpawnFromRegistry {}
    
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
    public abstract class SpawnRegistryBase<T> where T : class, IDespawnable
    {

    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private bool invokeEvent = true;
        public bool InvokeEvent => invokeEvent;

        [SerializeField]
        private SpawnedDictionary<T> spawned = new();
        public ISpawnedDictionary<T> Spawned => spawned;

    #endregion

    #region 이벤트

        public event Action<SpawnRegistryBase<T>, T> OnSpawn = null;
        public event Action<SpawnRegistryBase<T>, T> OnDespawn = null;

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 객체를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T Acquire();

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

            if (string.IsNullOrEmpty(spawnable.Key))
            {
                throw new InvalidOperationException("스폰될 객체의 키가 설정되어 있지 않습니다.");
            }

            if (spawned.ContainsKey(spawnable.Key))
            {
                throw new InvalidOperationException($"이미 동일 키({spawnable.Key})가 등록되어 있습니다.");
            }

            // 디스폰 메서드
            void DoDespawn() => Despawn(spawnable);

            spawned.Add(spawnable.Key, spawnable);

            spawnable.IsSpawned = true;
            spawnable.DespawnFromRegistry = DoDespawn;

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

            if (InvokeEvent)
            {
                OnSpawn?.Invoke(this, spawnable);
            }

            return spawnable;
        }

        private void Despawn(T despawnable)
        {
            if (despawnable == null)
            {
                throw new ArgumentNullException("디스폰할 객체를 설정해주세요.");
            }

            if (!despawnable.IsSpawned)
            {
                throw new InvalidOperationException("스폰되지 않은 객체를 디스폰할 수 없습니다.");
            }

            if (string.IsNullOrEmpty(despawnable.Key))
            {
                throw new InvalidOperationException("디스폰할 객체의 키가 설정되어 있지 않습니다.");
            }

            if (!spawned.ContainsKey(despawnable.Key))
            {
                throw new KeyNotFoundException($"등록되지 않은 키({despawnable.Key})에 해당하는 객체를 디스폰할 수 없습니다.");
            }

            DespawnInternal(despawnable);

            if (InvokeEvent)
            {
                OnDespawn?.Invoke(this, despawnable);
            }
        }

        private void DespawnInternal(T despawnable)
        {
            spawned.Remove(despawnable.Key);

            despawnable.IsSpawned = false;
            despawnable.DespawnFromRegistry = null;

            despawnable.OnDespawn();
        }

    #endregion

    }
    
    // ========================================================================
    /// <summary>
    /// 스폰을 관리하기 위한 클래스입니다.
    /// </summary>
    /// <typeparam name="T">스폰 가능한 객체의 타입입니다.</typeparam>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistry<T> : SpawnRegistryBase<T> where T : class, ISpawnable, IDespawnable
    {
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
    /// <typeparam name="T">스폰 가능한 객체의 타입입니다.</typeparam>
    /// <typeparam name="TParam">스폰 시 전달할 매개변수의 타입입니다.</typeparam>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistry<T, TParam> : SpawnRegistryBase<T> where T : class, ISpawnable<TParam>, IDespawnable
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn(TParam param)
        {
            var spawnable = Acquire();

            if (spawnable == null)
            {
                throw new InvalidOperationException("스폰할 객체를 가져올 수 없습니다.");
            }

            void OnSpawn(T spawnable) => spawnable.OnSpawn(param);

            return RegisterAndSpawn(spawnable, OnSpawn);
        }
    }
}