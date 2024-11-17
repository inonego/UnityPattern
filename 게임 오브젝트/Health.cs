using UnityEditor;
using UnityEngine;

public class Health : MonoBehaviour
{   

#region Enumerations

    public enum State
    {
        Alive, Dead, None
    }

#endregion

#region EventArgs

    public struct HPChangedEventArgs
    {
        public int PreviousHP;
        public int CurrentHP;
        public int Delta;
    }

    public struct HealedEventArgs
    {
        public int PreviousHP;
        public int CurrentHP;
        public int Delta;
        public int Value;
    }

    public struct DamagedEventArgs
    {
        public int PreviousHP;
        public int CurrentHP;
        public int Delta;
        public int Value;
    }

    public struct StateChangedEventArgs
    {
        public State Previous;
        public State Current;
    }

#endregion

#region Events

    public delegate void OnHPChangedEvent(HPChangedEventArgs e);
    public delegate void OnHealedEvent(HealedEventArgs e);
    public delegate void OnDamagedEvent(DamagedEventArgs e);
    public delegate void OnStateChangedEvent(StateChangedEventArgs e);
    
    /// <summary>
    /// 체력이 변경되었을때 호출되는 이벤트입니다.
    /// </summary>
    public event OnHPChangedEvent OnHPChanged;
    /// <summary>
    /// 힐을 받았을때 호출되는 이벤트입니다.
    /// </summary>
    public event OnHealedEvent OnHealed;
    /// <summary>
    /// 데미지를 입었을때 호출되는 이벤트입니다.
    /// </summary>
    public event OnDamagedEvent OnDamaged;
    /// <summary>
    /// 상태가 변화되었을떼 호출되는 이벤트입니다.
    /// </summary>
    public event OnStateChangedEvent OnStateChanged;

#endregion

    [field: SerializeField] public State Current { get; private set; } = State.None;

    public bool IsAlive => Current == State.Alive;
    public bool IsDead  => Current == State.Dead;

    [field: SerializeField] public bool AliveOnAwake  { get; set; } = true;
    [field: SerializeField] public bool DestroyOnDead { get; set; } = true;
    
    [field: SerializeField] public int HP       { get; private set; } = 0;
    [field: SerializeField] public int MaxHP    { get; private set; } = 0;

    private void Awake()
    {
        if (AliveOnAwake)
        {
            SetAlive();
        }
    }

    private void Update()
    {
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
    }

    private void Destroy()
    {
        #if UNITY_EDITOR
            if (!EditorApplication.isPlaying) return;
        #endif
        
        gameObject.Despawn();
    }
    
    private void SetState(State state)
    {
        State previous = Current; State current = state;

        Current = current;
        
        if (current == State.Alive)
        {
            SetHP(MaxHP);
        }
        
        OnStateChanged?.Invoke(new StateChangedEventArgs { Previous = previous, Current = current });

        if (current == State.Dead)
        {
            SetHP(0);

            if (DestroyOnDead)
            {
                Destroy();
            }
        }
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
        int previousHP = HP;

        HP = Mathf.Clamp(hp, 0, MaxHP);

        OnHPChanged?.Invoke(new HPChangedEventArgs { PreviousHP = previousHP, CurrentHP = HP, Delta = HP - previousHP });
    }

    /// <summary>
    /// 최대 체력을 설정합니다.
    /// </summary>
    /// <param name="maxHP">최대 체력 값</param>
    public void SetMaxHP(int maxHP)
    {
        if (maxHP < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("최대 체력 값은 0보다 작을 수 없습니다.");
            #endif
    
            maxHP = 0;
        }
        
        MaxHP = maxHP;

        SetHP(HP);
    }
    
    /// <summary>
    /// 힐(체력 회복)을 받습니다.
    /// </summary>
    /// <param name="heal">힐 값</param>
    public void TakeHeal(int heal)
    {
        // 죽은 상태면 힐을 받지 않습니다.
        if (IsDead) return;

        int previousHP = HP;

        if (heal < 0) {
            #if UNITY_EDITOR
                Debug.LogWarning("힐 값은 0보다 작을 수 없습니다.");
            #endif
    
            heal = 0;
        }
        
        // 체력을 회복합니다.
        SetHP(HP + heal);

        // 힐 이벤트 발생
        OnHealed?.Invoke(new HealedEventArgs { PreviousHP = previousHP, CurrentHP = HP, Delta = HP - previousHP, Value = heal });
    }

    /// <summary>
    /// 데미지(체력 감소)를 입습니다.
    /// </summary>
    /// <param name="damage">데미지 값</param>
    public void TakeDamage(int damage)
    {
        // 죽은 상태면 데미지를 입지 않습니다.
        if (IsDead) return;
        
        int previousHP = HP;

        if (damage < 0) 
        {
            #if UNITY_EDITOR
                Debug.LogWarning("데미지 값은 0보다 작을 수 없습니다.");
            #endif

            damage = 0;
        }
         
        // 체력을 깎습니다.
        SetHP(HP - damage);
        
        // 데미지 이벤트 발생
        OnDamaged?.Invoke(new DamagedEventArgs { PreviousHP = previousHP, CurrentHP = HP, Delta = HP - previousHP, Value = damage });
    }
}
