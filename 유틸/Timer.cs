using System;

using UnityEngine;

namespace inonego
{

using inonego.util;

// TODO
// 타이머 시간 조정 메서드 및 이벤트 추가
// 타이머 시간 무제한(스톱워치) 기능 추가

public class Timer
{

#region Enumerations

    public enum State
    {
        Started, Paused, Stopped
    }

#endregion

#region Events

    #region EventArgs

        public struct StateChangedEventArgs
        {
            public State Previous;
            public State Current;
        }        
        
        public struct EndedEventArgs
        {
            // 아직은 아무것도 없음
        }

    #endregion

    /// <summary>
    /// 상태가 변화되었을떼 호출되는 이벤트입니다.
    /// </summary>
    public event Action<Timer, StateChangedEventArgs> OnStateChanged;
    private Event<Timer, StateChangedEventArgs> OnStateChangedEvent = new();

    /// <summary>
    /// 타이머가 종료되었을때 호출되는 이벤트입니다.
    /// </summary>
    public event Action OnEnded;
    private Event<Timer, EndedEventArgs> OnEndedEvent = new();

#endregion

    [field: SerializeField] public State Current { get; private set; } = State.Stopped;

    public bool IsWorking => Current == State.Started;

    private float time      = 0f;
    private float current   = 0f;

    public float LeftTime       => current; 
    public float ElapsedTime    => time - current;
    public float LeftTime01     => LeftTime / time;
    public float ElapsedTime01  => ElapsedTime / time;

    private State previous  = State.Stopped;

    private void Clear()
    {
        previous = Current;
    }
    
    /// <summary>
    /// 타이머를 업데이트합니다.
    /// </summary>
    public void Update()
    {
        if (IsWorking)
        {
            current -= Time.deltaTime;

            if (current <= 0f)
            {
                Stop();
                
                OnEndedEvent.SetDirty();
            }
        }

        OnStateChangedEvent.InvokeIfDirty(this, new StateChangedEventArgs { Previous = previous, Current = Current });

        OnEndedEvent.InvokeIfDirty(this, new EndedEventArgs());

        Clear();
    }

    private void SetState(State state)
    {
        Current = state;

        OnStateChangedEvent.SetDirty();
    }

    /// <summary>
    /// 타이머의 작동을 시작시킵니다.
    /// </summary>
    /// <param name="time">시간 값</param>
    public void Start(float time)
    {
        SetState(State.Started);

        this.time = current = time;
    }

    /// <summary>
    /// 타이머의 작동을 중지시킵니다.
    /// </summary>
    public void Stop()
    {
        if (Current != State.Stopped)
        {
            SetState(State.Stopped);

            this.time = current = 0f;
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