using System;
using System.Collections;
using System.Collections.Generic;

namespace inonego
{
    using Serializable;

    public interface ISpawnedDictionary<TKey, T> : IReadOnlyDictionary<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey> {}

    public interface ISpawnRegistry<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey>
    {
        public ISpawnedDictionary<TKey, T> Spawned { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 해당 키를 가지는 객체를 찾습니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Find(TKey key);

        // ------------------------------------------------------------
        /// <summary>
        /// 동일 키를 가지는 객체를 찾습니다.
        /// </summary>
        // ------------------------------------------------------------
        public T Find(IKeyable<TKey> keyable);

        public event Action<TKey, T> OnSpawn;
        public event Action<TKey, T> OnDespawn;
    }
}