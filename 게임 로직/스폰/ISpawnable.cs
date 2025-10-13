using System;

namespace inonego
{
    public interface ISpawnable
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 서로 다른 객체끼리 구분하기 위한 키를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public string Key { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 객체가 스폰되었는지 여부를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool IsSpawned { get; protected internal set; }

        protected internal void OnSpawn();
        protected internal void OnDespawn();

        protected internal Action DespawnFromRegistry { get; set; }
    }
}