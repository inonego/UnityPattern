using UnityEngine;

using System;

namespace inonego
{
    using Serializable;

    [Serializable]
    public abstract partial class Entity : ISpawnRegistryObject<ulong>, IDeepCloneableFrom<Entity>
    {

    #region 키 설정

        [SerializeField, ReadOnly]
        protected XNullable<ulong> key = null;
        public ulong Key
        {
            get
            {
                if (key.HasValue)
                {
                    return key.Value;
                }

                throw new InvalidOperationException("키가 설정되어 있지 않습니다.");
            }
        }

        public bool HasKey => key.HasValue;

        protected internal virtual void SetKey(ulong key) => this.key = key;
        protected internal virtual void ClearKey() => this.key = null;

    #endregion

    #region 필드

        [SerializeField, ReadOnly]
        protected bool isSpawned = false;
        public bool IsSpawned => isSpawned;

        [SerializeField]
        protected Team lTeam = new Team();
        public Team Team => lTeam;

    #endregion
    
    #region 체력 관련

        [SerializeField]
        private HP hp = new HP();
        public HP HP => hp;

        private void InitHP()
        {
            if (hp == null)
            {
                throw new InvalidOperationException("체력이 설정되어 있지 않습니다.");
            }
            
            hp.OnStateChange += OnHPStateChange;

            if (hp.IsDead)
            {
                hp.MakeAlive();
            }
        }

        private void ReleaseHP()
        {
            if (hp == null)
            {
                throw new InvalidOperationException("체력이 설정되어 있지 않습니다.");
            }
            
            hp.OnStateChange -= OnHPStateChange;

            if (hp.IsAlive)
            {
                hp.MakeDead();
            }
        }
    
    #endregion

    #region 인터페이스 구현

        bool ISpawnRegistryObject<ulong>.IsSpawned { get => isSpawned; set => isSpawned = value; }

        Action IDespawnable.DespawnFromRegistry { get; set; }

        public virtual void OnBeforeSpawn()
        {
            InitHP();
        }

        public virtual void OnAfterDespawn()
        {
            ReleaseHP();
        }

    #endregion

    #region 생성자

        public Entity() : base() {}

        public Entity(HP hp) : this()
        {
            if (hp == null)
            {
                throw new ArgumentNullException("HP가 null입니다.");
            }
            
            this.hp = hp;
        }

    #endregion

    #region 복제 관련

        public virtual void CloneFrom(Entity source)
        {
            key = source.key;

            isSpawned = false;

            lTeam.CloneFrom(source.lTeam);

            hp.CloneFrom(source.hp);
        }
        
    #endregion

    #region 이벤트 핸들러   

        protected virtual void OnHPStateChange(object sender, ValueChangeEventArgs<HP.State> e)
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