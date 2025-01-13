using System;

using UnityEditor;
using UnityEngine;

// 체력 값의 타입을 정의합니다.
// 현재로서는 하나의 게임 안에서 한가지 타입만 사용할 수 있습니다.
// 향후 유니티가 C# 11.0을 지원하게 되면 Generic Math를 이용해 제너릭으로 적용할 예정입니다.
using TValue = System.Double;

namespace inonego
{

using inonego.util;

public class HP : MonoBehaviour
{   

#region 열거형 타입 정의

    public enum State        { Alive, Dead }

    private enum ApplyType   { Heal, Damage }

#endregion

#region 이벤트

    /// <summary>
    /// 상태가 변화되었을때 호출되는 이벤트입니다.
    /// </summary>
    protected Event<HP, StateChangedEventArgs> OnStateChangedEvent = new();
    public event Action<HP, StateChangedEventArgs> OnStateChanged { add => OnStateChangedEvent += value; remove => OnStateChangedEvent -= value; }

    public struct StateChangedEventArgs
    {
        public State Previous;
        public State Current;
    }
    
    /// <summary>
    /// 체력이 변경되었을때 호출되는 이벤트입니다.
    /// </summary>
    protected Event<HP, HPChangedEventArgs> OnHPChangedEvent = new();
    public event Action<HP, HPChangedEventArgs> OnHPChanged { add => OnHPChangedEvent += value; remove => OnHPChangedEvent -= value; }

    public struct HPChangedEventArgs
    {
        public TValue PreviousValue;
        public TValue CurrentValue;
        public TValue Delta;
    }
    
    /// <summary>
    /// 힐이나 데미지가 적용되었을때 호출되는 이벤트입니다.
    /// </summary>
    protected Event<HP, AppliedEventArgs> OnAppliedEvent = new();
    public event Action<HP, AppliedEventArgs> OnApplied { add => OnAppliedEvent += value; remove => OnAppliedEvent -= value; }

    public struct AppliedEventArgs
    {
        public TValue PreviousValue;
        public TValue CurrentValue;
        public TValue Delta;

        public TValue? Heal;
        public TValue? Damage;
    }

#endregion

#region 상태

    /// <summary>
    /// 현재 상태
    /// </summary>
    [field: SerializeField][HideInInspector] private State previous = State.Dead;
    [field: SerializeField] public State Current  { get; private set; } = State.Dead;

    /// <summary>
    /// 살아있는 상태인지 여부
    /// </summary>
    public bool IsAlive => Current == State.Alive;

    /// <summary>
    /// 죽어있는 상태인지 여부
    /// </summary>
    public bool IsDead  => Current == State.Dead;

    [field: SerializeField] public bool AliveOnAwake  { get; set; } = true;
    [field: SerializeField] public bool DestroyOnDead { get; set; } = true;

#endregion

#region 값

/*
    // 현재 값을 정수로 사용할지에 대한 여부입니다.
    private bool useAsInteger = false;
    public bool UseAsInteger
    {
        get => useAsInteger;
        set
        {
            useAsInteger = value; 
            
            SetMaxValue(MaxValue);
        }
    }
*/

    [field: SerializeField][HideInInspector] private TValue previousValue = default;
    [field: SerializeField] public TValue CurrentValue     { get; private set; } = default;
    [field: SerializeField] public TValue MaxValue         { get; private set; } = default;

    private TValue? heal       = null;
    private TValue? damage     = null;

#endregion

    private void Clear()
    {
        previous = Current;

        previousValue = CurrentValue;

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

        // 이전 상태를 재설정합니다.
        Clear();
    }

    protected virtual void Destroy() => Destroy(gameObject);

    /// <summary>
    /// 이벤트를 처리합니다.
    /// </summary>
    private void ProcessEvent()
    {
        // 체력이 회복되거나 데미지를 입었을때 체력을 설정하도록 합니다.
        if (heal is not null || damage is not null) SetValue(CurrentValue + (heal ?? default) - (damage ?? default));

        OnAppliedEvent.InvokeIfDirty(this, new AppliedEventArgs { PreviousValue = previousValue, CurrentValue = CurrentValue, Delta = CurrentValue - previousValue, Heal = heal, Damage = damage });

        OnHPChangedEvent.InvokeIfDirty(this, new HPChangedEventArgs { PreviousValue = previousValue, CurrentValue = CurrentValue, Delta = CurrentValue - previousValue });

        // 살아있는데
        if (IsAlive)
        {
            // HP가 0이면
            if (CurrentValue == default)
            {
                // 죽은 상태로 설정합니다.
                SetDead();
            }
        }

        OnStateChangedEvent.InvokeIfDirty(this, new StateChangedEventArgs { Previous = previous, Current = Current });

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

    /// <summary>
    /// 상태를 설정합니다.
    /// </summary>
    /// <param name="state">설정할 상태</param>
    private void SetState(State state)
    {
        Current = state;
        
        if (Current == State.Alive)
        {
            SetValue(MaxValue);
        }
        else
        //
        if (Current == State.Dead)
        {
            SetValue(default);
        }

        OnStateChangedEvent.SetDirty();
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
    public void SetValue(TValue hp)
    {
        CurrentValue = Math.Clamp(hp, default, MaxValue);

        OnHPChangedEvent.SetDirty();
    }

    /// <summary>
    /// 최대 체력을 설정합니다.
    /// </summary>
    /// <param name="maxHP">최대 체력 값</param>
    public void SetMaxValue(TValue value)
    {
        if (value < default(TValue)) {
            #if UNITY_EDITOR
                Debug.LogWarning("최대 체력 값은 0보다 작을 수 없습니다.");
            #endif
    
            value = default;
        }
        
        MaxValue = value;

        // 최대 체력이 변경됨에 따라 HP도 조정되도록 합니다.
        SetValue(CurrentValue);
    }
    
    /// <summary>
    /// 힐(체력 회복)를 적용시킵니다.
    /// </summary>
    /// <param name="value">적용할 값</param>
    public void ApplyHeal(TValue value)
    {
        Apply(ApplyType.Heal, value);
    }

    /// <summary>
    /// 데미지(체력 감소)를 적용시킵니다.
    /// </summary>
    /// <param name="value">적용할 값</param>
    public void ApplyDamage(TValue value)
    {
        Apply(ApplyType.Damage, value);
    }

    /// <summary>
    /// 힐(체력 회복) 또는 데미지(체력 감소)를 적용시킵니다.
    /// </summary>
    /// <param name="applyType">적용할 타입</param>
    /// <param name="value">적용할 값</param>
    private void Apply(ApplyType applyType, TValue value)
    {
        // 죽은 상태면 데미지나 힐을 받지 않습니다.
        if (IsDead) return;

        if (value < default(TValue)) {
            #if UNITY_EDITOR
                Debug.LogWarning("값이 0보다 작을 수 없습니다.");
            #endif
    
            value = default;
        }
        
        if (applyType == ApplyType.Heal)
        {
            if (heal is null) heal = default;

            heal += value;
        }
        else
        {
            if (damage is null) damage = default;

            damage += value; 
        }
        
        OnAppliedEvent.SetDirty();
    }
}

}