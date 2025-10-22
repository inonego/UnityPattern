using UnityEngine;

using inonego;

using System;

namespace inonego
{

    [Serializable]
    public abstract partial class Entity : ISpawnable, IDespawnable
    {

    #region 필드

        [SerializeField, ReadOnly]
        protected string key = Guid.NewGuid().ToString();
        public string Key => key;

        [SerializeField, ReadOnly]
        protected bool isSpawned = false;
        public bool IsSpawned => isSpawned;

        [SerializeField]
        protected Team lTeam = new Team();
        public Team Team => lTeam;

        [SerializeField]
        protected HP hp = null;
        public HP HP => hp;
    
    #endregion

    #region 인터페이스 구현

        bool ISpawnedFlag.IsSpawned { get => isSpawned; set => isSpawned = value; }

        Action IDespawnable.DespawnFromRegistry { get; set; }

        public virtual void OnSpawn()
        {
            hp.OnStateChange += OnHPStateChange;

            if (hp.IsDead)
            {
                hp.MakeAlive();
            }
        }

        public virtual void OnDespawn()
        {
            if (hp.IsAlive) 
            {
                hp.MakeDead();
            }

            hp.OnStateChange -= OnHPStateChange;
        }

    #endregion

    #region 생성자

        public Entity() : this(null) {}

        public Entity(HP hp)
        {
            this.hp = hp != null ? hp : new HP();
        }

    #endregion

    #region 이벤트 핸들러   

        protected virtual void OnHPStateChange(HP sender, ValueChangeEventArgs<HP.State> e)
        {
            if (e.Current == HP.State.Dead)
            {
                if (isSpawned)
                {
                    this.Despawn();
                }
            }
        }

    #endregion

    #region 메서드

        public virtual void ApplyDamage(int damage)
        {
            hp.ApplyDamage(damage);
        }

        public virtual void ApplyHeal(int amount)
        {
            hp.ApplyHeal(amount);
        }

    #endregion

    }
}