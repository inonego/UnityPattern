using UnityEngine;

using inonego;

using System;

namespace inonego
{

    [Serializable]
    public abstract partial class Entity : ISpawnable
    {

    #region 필드

        [SerializeField, ReadOnly]
        private string key = Guid.NewGuid().ToString();
        public string Key => key;

        [SerializeField, ReadOnly]
        private bool isSpawned = false;
        public bool IsSpawned => isSpawned;

        [SerializeField]
        private HP hp = new();
        public HP HP => hp;
    
    #endregion

    #region ISpawnable 인터페이스 구현

        Action ISpawnable.DespawnFromRegistry { get; set; }

        bool ISpawnable.IsSpawned { get => isSpawned; set => isSpawned = value; }

        void ISpawnable.OnSpawn()
        {
            hp.OnStateChange += OnHPStateChange;

            if (hp.IsDead)
            {
                hp.MakeAlive();
            }
        }

        void ISpawnable.OnDespawn()
        {
            if (hp.IsAlive) 
            {
                hp.MakeDead();
            }

            hp.OnStateChange -= OnHPStateChange;
        }

    #endregion

    #region 이벤트 핸들러   

        private void OnHPStateChange(HP sender, ValueChangeEventArgs<HP.State> e)
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

        public void GiveDamage(int damage)
        {
            hp.ApplyDamage(damage);
        }

        public void Heal(int amount)
        {
            hp.ApplyHeal(amount);
        }

    #endregion

    }
}