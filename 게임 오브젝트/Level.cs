using System;

using System.Collections.Generic;

using UnityEditor;
using UnityEngine;

namespace inonego
{

using inonego.util;

public class Level : MonoBehaviour
{
    
#region Events

    #region EventArgs

    public struct LevelUpEventArgs
    {
        public int PreviousValue;
        public int CurrentValue;
    }

    #endregion

    /// <summary>
    /// 레벨이 올랐을때 호출되는 이벤트입니다.
    /// </summary>
    public Event<Level, LevelUpEventArgs> OnLevelUpEvent = new();
    public event Action<Level, LevelUpEventArgs> OnLevelUp     { add => OnLevelUpEvent += value; remove => OnLevelUpEvent -= value; }

#endregion

    // 레벨이 0 레벨부터 시작입니다!!!

    [field: SerializeField] public int Value        { get; private set; } = 0;
    [field: SerializeField] public int Exp          { get; private set; } = 0;

    public int MaxValue => RequiredExps.Count;
    public int MaxExp => RequiredExps.Count > Value ? RequiredExps[Value] : 0;

    public List<int> RequiredExps = new List<int>();
    
    private void Update()
    {
        OnLevelUpEvent.InvokeIfDirty(this, new LevelUpEventArgs { PreviousValue = Value, CurrentValue = Value });
    }

    public void SetValue(int value)
    {
        Value = value;
    }

    public void SetExp(int exp)
    {
        Exp = exp;

        while (HasNextLevel && Exp >= RequiredExps[Value])
        {
            LevelUp();
        }
    }

    public bool HasNextLevel => Value < RequiredExps.Count - 1;

    public void AddEXP(int exp)
    {
        SetExp(Exp + exp);
    }

    public void LevelUp()
    { 
        SetValue(Value + 1);
            
        OnLevelUpEvent.SetDirty();
    }
}

}