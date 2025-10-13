using UnityEngine;

using inonego;

using System;

namespace inonego
{

    [Serializable]
    public abstract partial class MonoEntity : MonoBehaviour, ISpawnable
    {
        [SerializeReference]
        private Entity entity;
        public Entity Entity => entity;
        
        public void Init(Entity entity)
        {
            this.entity = entity;
        }

        public void Release()
        {
            entity = null;
        }
    }
}