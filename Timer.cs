using UnityEngine;

public class Timer
{
    public enum State
    {
        Started, Paused, Stopped
    }

    [field: SerializeField] public State Current { get; private set; } = State.Stopped;

    public bool IsWorking => Current == State.Started;

    private float time      = 0f;
    private float current   = 0f;

    public bool WasEndedThisFrame { get; private set; } = false;

    public float LeftTime       => current; 
    public float ElapsedTime    => time - current;
    public float LeftTime01     => LeftTime / time;
    public float ElapsedTime01  => ElapsedTime / time;

    public delegate void OnStateChangedEvent(State state);

    /// <summary>
    /// 상태가 변화되었을떼 호출되는 이벤트입니다.
    /// </summary>
    public event OnStateChangedEvent OnStateChanged;

    private void SetState(State state)
    {
        OnStateChanged?.Invoke(Current = state);
    }
    
    public void Start(float time)
    {
        WasEndedThisFrame = false;

        SetState(State.Started);

        this.time = current = time;
    } 

    public void Stop()
    {
        WasEndedThisFrame = false;

        SetState(State.Stopped);

        this.time = current = 0f;
    }

    public void Update()
    {
        WasEndedThisFrame = false;

        if (IsWorking)
        {
            current -= Time.deltaTime;

            if (current <= 0f)
            {
                Stop();
                
                WasEndedThisFrame = true;
            }
        }
    }

    public void Pause()
    {
        if (Current == State.Started)
        {
            SetState(State.Paused);
        }
    }

    public void Resume()
    {
        if (Current == State.Paused)
        {
            SetState(State.Started);
        }
    }
}