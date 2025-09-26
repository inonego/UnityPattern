using System;

using UnityEngine;

namespace inonego
{

// ============================================================
/// <summary>
/// 체력을 나타내는 클래스입니다.
/// </summary>
// ============================================================
[Serializable]
public class HP
{   
    // ------------------------------------------------------------
    /// <summary>
    /// 이벤트를 호출할지 여부를 결정합니다.
    /// </summary>
    // ------------------------------------------------------------
    public bool InvokeEvent = true;

    public enum ApplyRatioType
    {
        ByValue,
        ByMaxValue,
        ByMissingValue,
    }

    public enum State        { Alive, Dead }
    
    private enum ApplyType   { Heal, Damage }

#region 이벤트

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 체력 값이 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    // ------------------------------------------------------------
    public event ValueChangeEvent<HP, int> OnValueChange;

    // ------------------------------------------------------------
    /// <summary>
    /// 최대 체력 값이 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    // ------------------------------------------------------------
    public event ValueChangeEvent<HP, int> OnMaxValueChange;

    // ------------------------------------------------------------
    /// <summary>
    /// 상태가 변경될 때 호출되는 이벤트입니다.
    /// </summary>
    // ------------------------------------------------------------
    public event ValueChangeEvent<HP, State> OnStateChange;

    [Serializable]
    public struct ApplyEventArgs
    {
        public int Amount;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 힐이 적용되었을때 호출되는 이벤트입니다.
    /// </summary>
    // ------------------------------------------------------------
    public event Action<HP, ApplyEventArgs> OnHeal;

    // ------------------------------------------------------------
    /// <summary>
    /// 데미지가 적용되었을때 호출되는 이벤트입니다.
    /// </summary>
    // ------------------------------------------------------------
    public event Action<HP, ApplyEventArgs> OnDamage;

#endregion

#region 필드

    public bool IsAlive => current == State.Alive;
    public bool IsDead  => current == State.Dead;

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 상태(생,사)를 나타냅니다.
    /// </summary>
    // ------------------------------------------------------------
    [SerializeField]
    private State current = State.Dead;
    public State Current
    {
        get => current;
        protected set
        {
            var (prev, next) = (current, value);
            
            if (prev == next) return;
            
            current = next;

            Set(IsAlive ? maxValue : 0, autoChangeState: false);
      
            if (InvokeEvent)
            {
                OnStateChange?.Invoke(this, new ValueChangeEventArgs<State> { Previous = prev, Current = next });
            }
        }
    }
    
    // ------------------------------------------------------------
    /// <summary>
    /// 현재 체력 값입니다.
    /// </summary>
    // ------------------------------------------------------------
    [SerializeField]
    private int value = 0;
    public int Value
    {
        get => value;
        set => Set(value);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 최대 체력 값입니다.
    /// </summary>
    // ------------------------------------------------------------
    [SerializeField]
    private int maxValue = 0;
    public int MaxValue
    {
        get => maxValue;
        set => SetMax(value);
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 체력 비율 (0.0 ~ 1.0)
    /// </summary>
    // ------------------------------------------------------------
    public float Ratio => maxValue > 0 ? ((float)value / (float)maxValue) : 0f;
    
#endregion

#region 생성자 및 초기화

    public HP() {}

    public HP Clone()
    {
        var hp = new HP();
        CloneTo(hp);
        return hp;
    }

    public void CloneTo(HP target)
    {
        if (target == null)
        {
            throw new ArgumentNullException($"HP.CloneTo()의 인자가 null입니다.");
        }

        (target.current, target.value, target.maxValue) = (current, value, maxValue);
    }

#endregion

#region 메서드

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 상태를 살아있는 상태로 설정합니다.
    /// </summary>
    // ------------------------------------------------------------
    public void MakeAlive()
    {
        Current = State.Alive;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 상태를 죽어있는 상태로 설정합니다.
    /// </summary>
    // ------------------------------------------------------------
    public void MakeDead()
    {
        Current = State.Dead;
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 체력을 설정합니다.
    /// </summary>
    // ------------------------------------------------------------
    private void Set(int value, bool autoChangeState = true)
    {
        var (prev, next) = (this.value, Math.Clamp(value, 0, maxValue));

        if (prev == next) return;
        
        if (autoChangeState)
        {
            // 체력이 0 이하로 떨어지면 자동으로 죽음 상태로 변경
            if (prev > 0 && next <= 0)
            {
                MakeDead();
            }
            // 체력이 0에서 1 이상으로 올라가면 자동으로 생존 상태로 변경
            else if (prev <= 0 && next > 0)
            {
                MakeAlive();
            }
        }

        this.value = next;

        if (InvokeEvent)
        {
            OnValueChange?.Invoke(this, new ValueChangeEventArgs<int> { Previous = prev, Current = next });
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 최대 체력을 설정합니다.
    /// </summary>
    // ------------------------------------------------------------
    private void SetMax(int maxValue)
    {
        var (prev, next) = (this.maxValue, Math.Max(0, maxValue));

        if (prev == next) return;

        this.maxValue = next;

        // 현재 체력이 새로운 최대 체력을 초과하면 조정합니다.
        if (value > next)
        {
            Set(next);
        }

        if (InvokeEvent)
        {
            OnMaxValueChange?.Invoke(this, new ValueChangeEventArgs<int> { Previous = prev, Current = next });
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 힐(체력 회복)를 적용시킵니다.
    /// </summary>
    // ------------------------------------------------------------
    public void ApplyHeal(int amount)
    {
        // 죽은 상태면 힐을 받지 않습니다.
        if (IsDead) return;

        if (amount < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("값이 0보다 작을 수 없습니다.");
            #endif
    
            amount = 0;
        }
        else if (amount == 0) return;

        Set(value + amount);

        OnHeal?.Invoke(this, new ApplyEventArgs { Amount = amount });
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 데미지(체력 감소)을 적용시킵니다.
    /// </summary>
    // ------------------------------------------------------------
    public void ApplyDamage(int amount)
    {
        // 죽은 상태면 데미지를 받지 않습니다.
        if (IsDead) return;

        if (amount < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("값이 0보다 작을 수 없습니다.");
            #endif
    
            amount = 0;
        }
        else if (amount == 0) return;

        Set(value - amount);

        OnDamage?.Invoke(this, new ApplyEventArgs { Amount = amount });
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 적용할 양을 계산합니다.
    /// </summary>
    // ------------------------------------------------------------
    public int CalculateApplyAmount(float ratio, ApplyRatioType applyRatioType)
    {
        switch (applyRatioType)
        {
            case ApplyRatioType.ByValue:
                return (int)(value * ratio);
            case ApplyRatioType.ByMaxValue:
                return (int)(maxValue * ratio);
            case ApplyRatioType.ByMissingValue:
                return (int)((maxValue - value) * ratio);
        }
        return 0;
    }

#endregion

}

}

