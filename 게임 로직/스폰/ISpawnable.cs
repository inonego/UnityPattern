using System;

namespace inonego
{
    public interface ISpawnedFlag
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 객체가 스폰되었는지 여부를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool IsSpawned { get; protected internal set; }
    }

    // ========================================================================
    /// <summary>
    /// 스폰 가능한 객체를 위한 인터페이스입니다.
    /// </summary>
    // ========================================================================
    public interface ISpawnable : ISpawnedFlag
    {
        protected internal void OnBeforeSpawn();
    }

    // ========================================================================
    /// <summary>
    /// 스폰 가능한 객체를 위한 인터페이스입니다.
    /// </summary>
    /// <typeparam name="TParam">스폰 시 전달할 매개변수의 타입입니다.</typeparam>
    // ========================================================================
    public interface ISpawnable<TParam> : ISpawnedFlag
    {
        protected internal void OnBeforeSpawn(TParam param);
    }

    // ========================================================================
    /// <summary>
    /// 디스폰 가능한 객체를 위한 인터페이스입니다.
    /// </summary>
    // ========================================================================
    public interface IDespawnable : ISpawnedFlag
    {
        protected internal void OnAfterDespawn();

        protected internal Action DespawnFromRegistry { get; set; }
    }
}