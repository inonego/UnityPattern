using System;

using UnityEngine;

namespace inonego
{

// =======================================================================================
/// <summary>
/// <br/>게임에서 레벨과 경험치를 관리하기 위한 클래스입니다.
/// <br/>레벨은 0부터 시작하며, 최대 레벨은 ['레벨 업에 필요한 경험치' 목록의 개수] 입니다.
/// </summary>
// =======================================================================================
[Serializable]
public class Level
{
    // ------------------------------------------------------------
    /// <summary>
    /// 이벤트를 호출할지 여부를 결정합니다.
    /// </summary>
    // ------------------------------------------------------------
    public bool InvokeEvent = true;

    [SerializeField] private int current = 0;
    [SerializeField] private int exp = 0;

    // ------------------------------------------------------------
    /// <summary>
    /// 레벨 업을 제한할지를 결정합니다.
    /// </summary>
    // ------------------------------------------------------------
    public bool LockLevelUp = false;

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 레벨입니다.
    /// </summary>
    // ------------------------------------------------------------
    public int Current
    {
        get => current;
        set
        {
            current = Math.Clamp(value, 0, Max);

            exp = 0;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 최대로 도달할 수 있는 레벨의 값입니다.
    /// </summary>
    // ------------------------------------------------------------
    public int Max => RequiredExpToLevelUpArray.Length;

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 경험치입니다.
    /// </summary>
    // ------------------------------------------------------------
    public int Exp
    {
        get => exp;
        set
        { 
            int exp = value;

            if (exp >= MaxExp && !CanLevelUp)
            {
                exp = MaxExp;
            }
            else
            {
                while (exp >= MaxExp && CanLevelUp)
                {
                    exp -= MaxExp;
                    
                    LevelUp();
                }
            }

            this.exp = exp;
        }
    }

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 레벨에서 다음 레벨로 넘어갈 수 있는지를 반환합니다.
    /// </summary>
    // ------------------------------------------------------------
    public bool CanLevelUp => 0 <= current && current < Max && !LockLevelUp;

    // ------------------------------------------------------------------------------------------
    /// <summary>
    /// <br/>레벨업하는데 필요한 경험치 목록입니다.
    /// <br/>i번째 인덱스의 값은 레벨 (i)에서 레벨 (i + 1)로 넘어가기 위해 필요한 경험치를 의미합니다.
    /// </summary>
    // ------------------------------------------------------------------------------------------
    public int[] RequiredExpToLevelUpArray = new int[0];
    
    // ------------------------------------------------------------------------------------------
    /// <summary>
    /// 해당 레벨의 최대 경험치를 반환합니다.
    /// </summary>
    // ------------------------------------------------------------------------------------------
    public int GetRequiredExpToLevelUp(int level) => 0 <= level && level < Max ? RequiredExpToLevelUpArray[level] : 0;

    // ------------------------------------------------------------
    /// <summary>
    /// 현재 레벨의 최대 경험치를 반환합니다.
    /// </summary>
    /// ------------------------------------------------------------
    public int MaxExp => GetRequiredExpToLevelUp(current);
    
#region 이벤트

    [Serializable]
    public struct LevelUpEventArgs
    {
        public int Value;
    }

    public event Action<Level, LevelUpEventArgs> OnLevelUp = null;

#endregion

    // ------------------------------------------------------------
    /// <summary>
    /// 레벨 업 시킵니다.
    /// </summary>
    // ------------------------------------------------------------
    public void LevelUp(int amount = 1)
    {   
        for (int i = 0; i < amount; i++)
        {
            if (CanLevelUp)
            {
                Current++;

                if (InvokeEvent)
                {
                    OnLevelUp?.Invoke(this, new LevelUpEventArgs { Value = current });
                }
            }
            else
            {
                break;
            }
        }
    }
}

}