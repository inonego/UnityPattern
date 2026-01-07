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

        // --------------------------------------------------------------------------------
        /// <summary>
        /// 디스폰 시 죽음 상태로 설정합니다.
        /// </summary>
        // --------------------------------------------------------------------------------
        public bool MakeDeadOnDespawn = true;

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
        private HP hp = null;
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
            
            // --------------------------------------------------------------------------------
            // 먼저 이벤트를 해제함으로써, 디스폰이 중복으로 호출되는 것을 방지합니다.
            // --------------------------------------------------------------------------------
            hp.OnStateChange -= OnHPStateChange;

            // --------------------------------------------------------------------------------
            // 그 다음에 체력을 죽음 상태로 설정합니다.
            // --------------------------------------------------------------------------------
            if (MakeDeadOnDespawn)
            {
                if (hp.IsAlive)
                {
                    hp.MakeDead();
                }
            }
        }
    
    #endregion

    #region 인터페이스 구현

        bool ISpawnRegistryObject<ulong>.IsSpawned { get => isSpawned; set => isSpawned = value; }

        Action IDespawnable.DespawnFromRegistry { get; set; }

        public virtual void OnBeforeSpawn()
        {
            // NONE
        }

        void ISpawnable.OnAfterSpawn()
        {
            InitHP();
        }

        void IDespawnable.OnBeforeDespawn()
        {
            ReleaseHP();
        }

        public virtual void OnAfterDespawn()
        {
            // NONE
        }

    #endregion

    #region 생성자

        public Entity(HP overrideHP = null)
        {
            if (overrideHP != null)
            {
                this.hp = overrideHP;
            }
            else
            {
                this.hp = new HP();
            }
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

        public virtual bool ApplyDamage(int damage, Entity source = null)
        {
            if (damage < 0)
            {
                Debug.LogError("데미지 값이 0보다 작을 수 없습니다.");
                return false;
            }

            hp.ApplyDamage(damage);

            return true;
        }

        public virtual bool ApplyHeal(int amount, Entity source = null)
        {
            if (amount < 0)
            {
                Debug.LogError("힐 값이 0보다 작을 수 없습니다.");
                return false;
            }

            hp.ApplyHeal(amount);

            return true;
        }

    #endregion

    }
}