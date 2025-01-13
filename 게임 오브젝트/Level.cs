using System;

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace inonego
{

using inonego.util;

public class Level : MonoBehaviour
{

#region 이벤트

    /// <summary>
    /// 레벨이 올랐을때 호출되는 이벤트입니다.
    /// </summary>
    protected Event<Level, LevelUpEventArgs> OnLevelUpEvent = new();
    public event Action<Level, LevelUpEventArgs> OnLevelUp { add => OnLevelUpEvent += value; remove => OnLevelUpEvent -= value; }

    public struct LevelUpEventArgs
    {
        public int PreviousValue;
        public int CurrentValue;

        public int PreviousExp;
        public int CurrentExp;
    }

#endregion
    
    // 레벨이 0 레벨부터 시작입니다!!!
    [field: SerializeField][HideInInspector] private int previousValue = 0;
    [field: SerializeField] public int CurrentValue         { get; private set; } = 0;

    [field: SerializeField][HideInInspector] private int previousExp = 0;
    [field: SerializeField] public int CurrentExp           { get; private set; } = 0;

    /// <summary>
    /// 레벨업하는데 필요한 경험치 목록입니다.
    /// </summary>
    [field: SerializeField] public List<int> RequiredExps   { get; private set; } = new List<int>();

    /// <summary>
    /// 최대 레벨을 반환합니다.
    /// </summary>
    public int MaxValue => RequiredExps.Count - 1;

    /// <summary>
    /// 현재 레벨의 다음 레벨이 있는지를 반환합니다.
    /// </summary>
    public bool CanLevelUp => CurrentValue < MaxValue;

    /// <summary>
    /// 현재 레벨의 최대 경험치를 반환합니다.
    /// </summary>
    public int MaxExpOnCurrentValue => RequiredExps.Count > CurrentValue ? RequiredExps[CurrentValue] : 0;
    
    private void Update()
    {
        ProcessEvent();

        // 이전 상태를 재설정합니다.
        Clear();
    }
    
    /// <summary>
    /// 이벤트를 처리합니다.
    /// </summary>
    private void ProcessEvent()
    {
        OnLevelUpEvent.InvokeIfDirty(this, new LevelUpEventArgs { PreviousValue = previousValue, CurrentValue = CurrentValue, PreviousExp = previousExp, CurrentExp = CurrentExp });
    }

    private void Clear()
    {
        previousValue = CurrentValue;
        
        previousExp = CurrentExp;
    }

    public void SetValue(int value)
    {
        if (!(0 <= value && value <= MaxValue))
        {
            #if UNITY_EDITOR
                Debug.LogWarning($"레벨이 범위(0 - {MaxValue})를 벗어났습니다.");
            #endif
        }

        CurrentValue = Math.Clamp(value, 0, MaxValue);

        CurrentExp = 0;
    }

    public void SetExp(int exp)
    {
        while (CanLevelUp && exp >= RequiredExps[CurrentValue])
        {   
            LevelUp();

            exp -= RequiredExps[CurrentValue];
        }

        CurrentExp = exp;
    }

    public void AddEXP(int exp)
    {
        SetExp(CurrentExp + exp);
    }

    public void LevelUp()
    { 
        if (CanLevelUp)
        {
            SetValue(CurrentValue + 1);

            OnLevelUpEvent.SetDirty();
        }
    }
}

}