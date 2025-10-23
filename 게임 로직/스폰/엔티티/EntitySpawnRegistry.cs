using System;

using UnityEngine;

namespace inonego
{
    [Serializable]
    public abstract class EntitySpawnRegistry<TEntity> : SpawnRegistry<ulong, TEntity>
    where TEntity : Entity
    {

    #region 필드

        [SerializeReference]
        private IKeyGenerator<ulong> keyGenerator = new IncreKeyGenerator();
        public IKeyGenerator<ulong> KeyGenerator => keyGenerator;

    #endregion

    #region 생성자

        public EntitySpawnRegistry() : base() {}

        public EntitySpawnRegistry(IKeyGenerator<ulong> keyGenerator) : this()
        {
            if (keyGenerator == null)
            {
                throw new ArgumentNullException("KeyGenerator가 null입니다.");
            }

            this.keyGenerator = keyGenerator;
        }

        #endregion

        #region 메서드

        protected override void OnBeforeSpawn(TEntity spawnable)
        {
            base.OnBeforeSpawn(spawnable);

            // 스폰될때 키를 생성하여 엔티티에 설정합니다.
            var key = keyGenerator.Generate();

            spawnable.SetKey(key);
        }

    #endregion

    }
}