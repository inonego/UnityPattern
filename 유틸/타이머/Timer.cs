using System;

using UnityEngine;

using TValue = System.Double;

namespace inonego
{

using inonego.util;

[Serializable]
public class Timer
{
    [Serializable]
    public class DATA
    {
        [field: SerializeField] public TValue Target     { get; set; } = default;
        [field: SerializeField] public TValue Current    { get; set; } = default;

        public TValue LeftTime       => Target - Current;
        public TValue ElapsedTime    => Current;

        public TValue LeftTime01     => LeftTime / Target;
        public TValue ElapsedTime01  => ElapsedTime / Target;
    }

#region 열거형 타입 정의

    public enum State
    {
        Started, Paused, Stopped
    }

#endregion

#region 이벤트
        
    /// <summary>
    /// 상태가 변화되었을떼 호출되는 이벤트입니다.
    /// </summary>
    public Event<Timer, StateChangedEventArgs> OnStateChanged = new();

    public struct StateChangedEventArgs
    {
        public State Previous;
        public State Current;
    }        

    /// <summary>
    /// 타이머가 종료되었을때 호출되는 이벤트입니다.
    /// </summary>
    public Event<Timer, EndedEventArgs> OnEnded = new();

    public struct EndedEventArgs
    {
        // TODO
    }

#endregion

#region 상태

    [field: SerializeField] public State Current    { get; private set; } = State.Stopped;
    [field: SerializeField] public State Previous   { get; private set; } = State.Stopped;
    
    public bool IsWorking => Current == State.Started;

#endregion

#region 값

    [field: SerializeField] public DATA Time { get; private set; } = new();

#endregion

    private void Clear()
    {
        Previous = Current;
    }
    
    /// <summary>
    /// 타이머를 업데이트합니다.
    /// </summary>
    public void Update()
    {
        if (IsWorking)
        {
            Time.Current += UnityEngine.Time.deltaTime;
            
            // 타이머의 시간이 목표 시간을 초과하면 타이머를 종료합니다.
            if (Time.Current >= Time.Target)
            {
                Stop();
                
                OnEnded.SetDirty();
            }
        }

        OnStateChanged.InvokeIfDirty(this, new StateChangedEventArgs { Previous = Previous, Current = Current });

        OnEnded.InvokeIfDirty(this, new EndedEventArgs { });

        Clear();
    }

    private void SetState(State state)
    {
        Current = state;

        OnStateChanged.SetDirty();
    }

    /// <summary>
    /// 타이머의 작동을 시작시킵니다.
    /// </summary>
    public void Start()
    {
        Time.Current = default;

        SetState(State.Started);
    }

    /// <summary>
    /// 타이머의 작동을 시작시킵니다.
    /// </summary>
    /// <param name="time">목표 시간 값</param>
    public void Start(TValue time)
    {
        Time.Target = time;

        Start();
    }

    /// <summary>
    /// 타이머의 작동을 중지시킵니다.
    /// </summary>
    public void Stop()
    {
        if (Current != State.Stopped)
        {
            Time.Current = default;

            SetState(State.Stopped);
        }
    }

    /// <summary>
    /// 타이머의 작동을 일시정지시킵니다.
    /// </summary>
    public void Pause()
    {
        if (Current == State.Started)
        {
            SetState(State.Paused);
        }
    }

    /// <summary>
    /// 타이머의 작동을 재개시킵니다.
    /// </summary>
    public void Resume()
    {
        if (Current == State.Paused)
        {
            SetState(State.Started);
        }
    }
}

}