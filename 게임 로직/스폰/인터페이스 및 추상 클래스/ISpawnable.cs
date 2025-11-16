using System;

namespace inonego
{
    // ========================================================================
    /// <summary>
    /// 스폰 등록 가능한 객체를 위한 인터페이스입니다.
    /// </summary>
    /// <typeparam name="TKey">스폰 등록 가능한 객체의 키 타입입니다.</typeparam>
    // ========================================================================
    public interface ISpawnRegistryObject<TKey> : IKeyable<TKey>, ISpawnable, IDespawnable
    where TKey : IEquatable<TKey>
    {
        public bool IsSpawned 
        {
            get; protected internal set;
        }
    }

    // ========================================================================
    /// <summary>
    /// 스폰 가능한 객체를 위한 인터페이스입니다.
    /// </summary>
    // ========================================================================
    public interface ISpawnable
    {
        public void OnBeforeSpawn();
    }

    // ========================================================================
    /// <summary>
    /// 디스폰 가능한 객체를 위한 인터페이스입니다.
    /// </summary>
    // ========================================================================
    public interface IDespawnable
    {
        public void OnAfterDespawn();

        protected internal Action DespawnFromRegistry { get; set; }
    }
}