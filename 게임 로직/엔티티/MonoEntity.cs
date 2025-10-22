using UnityEngine;

using inonego;

using System;

namespace inonego
{

    [Serializable]
    public abstract partial class MonoEntity : MonoBehaviour, ISpawnable<Entity>, IDespawnable
    {
        
    #region 필드

        public string Key => entity != null ? entity.Key : null;

        [SerializeField, ReadOnly]
        protected bool isSpawned = false;
        public bool IsSpawned => isSpawned;

        [SerializeReference]
        protected Entity entity;
        public Entity Entity => entity;
        
    #endregion

    #region 인터페이스 구현
        
        bool ISpawnedFlag.IsSpawned { get => isSpawned; set => isSpawned = value; }

        Action IDespawnable.DespawnFromRegistry { get; set; }
        
        public void OnSpawn(Entity entity)
        {
            this.entity = entity;
        }

        public void OnDespawn()
        {
            this.entity = null;
        }

    #endregion

    }
}