using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    using Serializable;

    [Serializable]
    public class SpawnedDictionary<T> : XDictionary<string, SerializeReferenceWrapper<T>>, ISpawnedDictionary<T> where T : class, ISpawnable { }
    public interface ISpawnedDictionary<T> : IReadOnlyDictionary<string, SerializeReferenceWrapper<T>> where T : class, ISpawnable { }
    
    public static class SpawnRegistryUtility
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 디스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void Despawn(this ISpawnable spawnable)
        {
            if (spawnable != null)
            {
                if (!spawnable.IsSpawned)
                {
                    throw new InvalidOperationException("스폰되지 않은 객체를 디스폰할 수 없습니다.");
                }

                spawnable.DespawnFromRegistry?.Invoke();
            }
        }
    }

    // ========================================================================
    /// <summary>
    /// 스폰 등록을 관리하기 위한 클래스입니다.
    /// </summary>
    // ========================================================================
    [Serializable]
    public abstract class SpawnRegistry<T> where T : class, ISpawnable
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

        public event Action<SpawnRegistry<T>, T> OnSpawn = null;
        public event Action<SpawnRegistry<T>, T> OnDespawn = null;

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
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn()
        {
            var spawnable = Acquire();

            if (spawnable == null)
            {
                throw new NullReferenceException("스폰을 진행하는 중에 객체를 가져올 수 없습니다.");
            }
            
            if (spawnable.IsSpawned)
            {
                throw new InvalidOperationException("이미 스폰된 객체입니다.");
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

            spawnable.OnSpawn();
            
            if (InvokeEvent)
            {
                OnSpawn?.Invoke(this, spawnable);
            }

            return spawnable;
        }

        private void Despawn(T spawnable)
        {
            if (spawnable == null)
            {
                throw new ArgumentNullException("디스폰할 객체를 설정해주세요.");
            }

            if (!spawnable.IsSpawned)
            {
                throw new InvalidOperationException("스폰되지 않은 객체를 디스폰할 수 없습니다.");
            }

            if (!spawned.ContainsKey(spawnable.Key))
            {
                throw new InvalidOperationException($"등록되지 않은 키({spawnable.Key})에 해당하는 객체를 디스폰할 수 없습니다.");
            }

            spawned.Remove(spawnable.Key);

            spawnable.IsSpawned = false;
            spawnable.DespawnFromRegistry = null;

            spawnable.OnDespawn();

            if (InvokeEvent)
            {
                OnDespawn?.Invoke(this, spawnable);
            }
        }

    #endregion

    }
}