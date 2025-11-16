using System;
using System.Collections;
using System.Collections.Generic;

namespace inonego
{
    using Serializable;

    public interface ISpawnedDictionary<TKey, T> : IReadOnlyDictionary<TKey, SerializeReferenceWrapper<T>>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey> {}

    public interface ISpawnRegistry<TKey, T>
    where TKey : IEquatable<TKey>
    where T : class, ISpawnRegistryObject<TKey>
    {
        public ISpawnedDictionary<TKey, T> Spawned { get; }

        public event Action<TKey, T> OnSpawn;
        public event Action<TKey, T> OnDespawn;
    }
}