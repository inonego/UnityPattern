using UnityEngine;

public class Health : MonoBehaviour
{   
    [field: SerializeField] public bool IsAlive { get; private set; } = false;
    public bool IsDead => !IsAlive;

    [field: SerializeField] public int HP       { get; private set; } = 0;
    [field: SerializeField] public int MaxHP    { get; private set; } = 0;
    
    public delegate void OnHealedEvent(int heal);
    public delegate void OnDamagedEvent(int damage);
    public delegate void OnDiedEvent();
     
    /// <summary>
    /// 힐을 받았을때 호출되는 이벤트입니다.
    /// </summary>
    public event OnHealedEvent OnHealed;
    /// <summary>
    /// 데미지를 입었을때 호출되는 이벤트입니다.
    /// </summary>
    public event OnDamagedEvent OnDamaged;
    /// <summary>
    /// 죽었을때 호출되는 이벤트입니다.
    /// </summary>
    public event OnDiedEvent OnDied;
    
/// <summary>
/// 힐(체력 회복)을 받습니다.
/// </summary>
/// <param name="heal">힐 값</param>
    public void TakeHeal(int heal)
    {
        if (heal < 0) {
        #if UNITY_EDITOR
            Debug.LogWarning("힐 값은 0보다 작을 수 없습니다.");
        #endif
    
            heal = 0;
        }

        // 죽은 상태면 힐을 받지 않습니다.
        if (IsDead) return;
        
        // 체력을 회복합니다.
        SetHP(HP + heal);

        // 힐 이벤트 발생
        OnHealed?.Invoke(heal);
    }

/// <summary>
/// 데미지(체력 감소)를 입습니다.
/// </summary>
/// <param name="damage">데미지 값</param>
    public void TakeDamage(int damage)
    {
        if (damage < 0) 
        {
        #if UNITY_EDITOR
            Debug.LogWarning("데미지 값은 0보다 작을 수 없습니다.");
        #endif

            damage = 0;
        }
         
        // 죽은 상태면 데미지를 입지 않습니다.
        if (IsDead) return;
        
        // 체력을 깎습니다.
        SetHP(HP - damage);

        if (HP == 0)
        {
            // 체력이 0이 되면 죽습니다.
            Die();

            return;
        }

        // 데미지 이벤트 발생
        OnDamaged?.Invoke(damage);
    }

/// <summary>
/// 체력을 설정합니다.
/// </summary>
/// <param name="hp">체력</param>
    public void SetHP(int hp){
        HP = Mathf.Clamp(hp, 0, MaxHP);
    }

/// <summary>
/// 최대 체력을 설정합니다.
/// </summary>
/// <param name="maxHP">최대 체력</param>
    public void SetMaxHP(int maxHP){
        MaxHP = maxHP;

        SetHP(HP);
    }

/// <summary>
/// 현재 상태를 살아있는 상태로 설정합니다.
/// </summary>
    public void SetAlive()
    {
        HP = MaxHP;

        IsAlive = true;
    }

/// <summary>
/// 현재 상태를 죽어있는 상태로 설정합니다.
/// </summary>
    public void Die() {
        HP = 0;

        IsAlive = false;

        OnDied?.Invoke();
    }
}
