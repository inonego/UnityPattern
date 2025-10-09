using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego
{
    public static class SpawnRegistryUtility
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 디스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void Despawn(this ISpawnable spawnable)
        {
            spawnable?.DespawnFromRegistry?.Invoke();
        }
    }

    [Serializable]
    public class SpawnRegistry<T> where T : class, ISpawnable
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

        [SerializeReference]
        private IPool<T> pool = null;

        public IReadOnlyCollection<T> Spawned => pool.Acquired;
        public IReadOnlyCollection<T> Despawned => pool.Released;

    #endregion

    #region 이벤트

        public event Action<SpawnRegistry<T>, T> OnSpawn = null;
        public event Action<SpawnRegistry<T>, T> OnDespawn = null;

    #endregion

    #region 생성자 및 초기화

        public SpawnRegistry()
        {
            pool = new Pool<T>();
        }

        public SpawnRegistry(IPool<T> pool)
        {
            if (pool == null)
            {
                throw new ArgumentNullException("Pool은 반드시 초기화되어야 합니다. 생성자에서 초기화해주세요.");
            }

            this.pool = pool;
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 객체를 스폰합니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Spawn()
        {
            var spawnable = pool.Acquire();

            // 디스폰 메서드
            void DoDespawn() => Despawn(spawnable);

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
                throw new ArgumentNullException();
            }

            if (InvokeEvent)
            {
                OnDespawn?.Invoke(this, spawnable);
            }

            spawnable.OnDespawn();
            spawnable.DespawnFromRegistry = null;

            pool.Release(spawnable);
        }

    #endregion

    }
}