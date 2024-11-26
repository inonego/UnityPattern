// #define USE_DEFAULT_DESTROY

using System;

using UnityEditor;
using UnityEngine;

namespace inonego
{

public partial class Health : MonoBehaviour
{   

#region Enumerations

    public enum State       { Alive, Dead, None }

    private enum ApplyType   { Heal, Damage }

#endregion

#region Events

    public delegate void EventHandler();
    public delegate void EventHandler<TEventArgs>(Health sender, TEventArgs e);

    [Flags]
    private enum EventFlag
    {
        StateChanged    = 1 << 0,
        HPChanged       = 1 << 1,
        Applied         = 1 << 2,
    }

    private class Event
    {   
        public EventFlag Flag;

        public bool HasFlag(EventFlag eventFlag) => Flag.HasFlag(eventFlag);

        public void Clear() => Flag = 0;
    }

    private Event @event;

    #region EventArgs

        public struct StateChangedEventArgs
        {
            public State Previous;
            public State Current;
        }

        public struct HPChangedEventArgs
        {
            public int PreviousHP;
            public int CurrentHP;
            public int Delta;
        }

        public struct AppliedEventArgs
        {
            public int PreviousHP;
            public int CurrentHP;
            public int Delta;

            public int? Heal;
            public int? Damage;
        }

    #endregion

    /// <summary>
    /// 상태가 변화되었을때 호출되는 이벤트입니다.
    /// </summary>
    public event EventHandler<StateChangedEventArgs> OnStateChanged;

    /// <summary>
    /// 체력이 변경되었을때 호출되는 이벤트입니다.
    /// </summary>
    public event EventHandler<HPChangedEventArgs> OnHPChanged;
    /// <summary>
    /// 힐이나 데미지가 적용되었을때 호출되는 이벤트입니다.
    /// </summary>
    public event EventHandler<AppliedEventArgs> OnHealDamageApplied;

#endregion

    [field: SerializeField] public State Current { get; private set; } = State.None;

    public bool IsAlive => Current == State.Alive;
    public bool IsDead  => Current == State.Dead;

    [field: SerializeField] public bool AliveOnAwake  { get; set; } = true;
    [field: SerializeField] public bool DestroyOnDead { get; set; } = true;
    
    [field: SerializeField] public int HP       { get; private set; } = 0;
    [field: SerializeField] public int MaxHP    { get; private set; } = 0;

    private State previous  = State.None;
    private int previousHP  = 0;
    private int? heal       = null;
    private int? damage     = null;

    private void Clear()
    {
        @event.Clear();

        previous   = Current;
        previousHP = HP;
        heal       = null;
        damage     = null;
    }

    private void Awake()
    {
        if (AliveOnAwake)
        {
            SetAlive();
        }
    }

    private void Update()
    {
        ProcessEvent();

        Clear();
    }

    #if USE_DEFAULT_DESTROY
        // 기본적으로 게임 오브젝트를 파괴합니다.
        private void Destroy() => Destroy(gameObject);
    #else
        // 다른 곳에서 Destroy에 대해서 정의하는 경우에만 사용하세요.
        private partial void Destroy();
    #endif

    private void ProcessEvent()
    {
        if (heal is not null || damage is not null) SetHP(HP + (heal ?? 0) - (damage ?? 0));

        if (@event.HasFlag(EventFlag.Applied))
        {   
            OnHealDamageApplied?.Invoke(this, new AppliedEventArgs { PreviousHP = previousHP, CurrentHP = HP, Delta = HP - previousHP, Heal = heal, Damage = damage });
        }
       
        if (@event.HasFlag(EventFlag.HPChanged))
        {
            OnHPChanged?.Invoke(this, new HPChangedEventArgs { PreviousHP = previousHP, CurrentHP = HP, Delta = HP - previousHP });
        }

        // 살아있는데
        if (IsAlive)
        {
            // HP가 0이면
            if (HP == 0)
            {
                // 죽은 상태로 설정합니다.
                SetDead();
            }
        }

        if (@event.HasFlag(EventFlag.StateChanged))
        {
            OnStateChanged?.Invoke(this, new StateChangedEventArgs { Previous = previous, Current = Current });
        }

        if (IsDead)
        {
            #if UNITY_EDITOR
                if (!EditorApplication.isPlaying) return;
            #endif
        
            if (DestroyOnDead)
            {
                Destroy();
            }
        }
    }

    private void SetState(State state)
    {
        Current = state;
        
        if (Current == State.Alive)
        {
            SetHP(MaxHP);
        }
        else
        //
        if (Current == State.Dead)
        {
            SetHP(0);
        }

        @event.Flag |= EventFlag.StateChanged;
    }

    /// <summary>
    /// 현재 상태를 살아있는 상태로 설정합니다.
    /// </summary>
    public void SetAlive()
    {
        SetState(State.Alive);
    }

    /// <summary>
    /// 현재 상태를 죽어있는 상태로 설정합니다.
    /// </summary>
    public void SetDead()
    {
        SetState(State.Dead);
    }

    /// <summary>
    /// 체력을 설정합니다.
    /// </summary>
    /// <param name="hp">체력 값</param>
    public void SetHP(int hp)
    {
        HP = Mathf.Clamp(hp, 0, MaxHP);

        @event.Flag |= EventFlag.HPChanged;
    }

    /// <summary>
    /// 최대 체력을 설정합니다.
    /// </summary>
    /// <param name="maxHP">최대 체력 값</param>
    public void SetMaxHP(int value)
    {
        if (value < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("최대 체력 값은 0보다 작을 수 없습니다.");
            #endif
    
            value = 0;
        }
        
        MaxHP = value;

        // 최대 체력이 변경됨에 따라 HP도 조정되도록 합니다.
        SetHP(HP);
    }
    
    /// <summary>
    /// 힐(체력 회복)를 적용시킵니다.
    /// </summary>
    /// <param name="value">적용할 값</param>
    public void ApplyHeal(int value)
    {
        Apply(ApplyType.Heal, value);
    }

    /// <summary>
    /// 데미지(체력 감소)를 적용시킵니다.
    /// </summary>
    /// <param name="value">적용할 값</param>
    public void ApplyDamage(int value)
    {
        Apply(ApplyType.Damage, value);
    }

    /// <summary>
    /// 힐(체력 회복) 또는 데미지(체력 감소)를 적용시킵니다.
    /// </summary>
    /// <param name="applyType">적용할 타입</param>
    /// <param name="value">적용할 값</param>
    private void Apply(ApplyType applyType, int value)
    {
        // 죽은 상태면 데미지나 힐을 받지 않습니다.
        if (IsDead) return;

        if (value < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("값이 0보다 작을 수 없습니다.");
            #endif
    
            value = 0;
        }
        
        if (applyType == ApplyType.Heal)
        {
            if (heal is null) heal = 0;

            heal += value;
        }
        else
        {
            if (damage is null) damage = 0;

            damage += value; 
        }
        
        @event.Flag |= EventFlag.Applied;
    }
}

}