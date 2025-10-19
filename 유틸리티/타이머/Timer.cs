using System;

using UnityEngine;

using TValue = System.Single;

namespace inonego
{
    public enum TimerState { Ready, Run, Pause }

    public delegate void TimerEndEvent<in TSender>(TSender sender, TimerEndEventArgs e);

    [Serializable]
    public struct TimerEndEventArgs
    {
        // NONE
    }

    public interface ITimerEventHandler<out TSelf>
    {
        public InvokeEventFlag InvokeEvent { get; }

        public event TimerEndEvent<TSelf> OnEnd;
        public event ValueChangeEvent<TSelf, TimerState> OnStateChange;
    }

    // ==================================================================
    /// <summary>
    /// <br/>타이머 클래스입니다.
    /// <br/>업데이트 메서드를 통해서, 타이머의 작동 시간을 업데이트해야합니다.
    /// </summary>
    // ==================================================================
    [Serializable]
    public partial class Timer : ITimer, IReadOnlyTimer, ITimerEventHandler<Timer>
    {
        [SerializeField]
        private InvokeEventFlag invokeEvent = new();
        public InvokeEventFlag InvokeEvent => invokeEvent;

        [SerializeField] private TValue duration = default;
        [SerializeField] private TValue elapsedTime = default;
        
        // --- Editor용 캐시된 시작 Duration ---
        [SerializeField] internal TValue cachedStartDuration;

        public TValue Duration 
        { 
            get => duration; 
            set 
            {
                var next = value;

                if (!IsRunning && !IsPaused)
                {
                    throw new InvalidOperationException("Duration은 Run 또는 Pause 상태일 때만 변경할 수 있습니다.");
                }
                
                if (next < 0.0f)
                {
                    throw new InvalidTimeException("Duration은 0 이상이어야 합니다.");
                }
                
                duration = next;
                
                elapsedTime = Mathf.Clamp(elapsedTime, 0.0f, duration);
            }
        }

        public TValue ElapsedTime 
        { 
            get => elapsedTime;
            set
            {
                var next = value;
                
                if (!IsRunning && !IsPaused)
                {
                    throw new InvalidOperationException("ElapsedTime은 Run 또는 Pause 상태일 때만 변경할 수 있습니다.");
                }

                if (next < 0.0f)
                {
                    throw new InvalidTimeException("ElapsedTime은 0 이상이어야 합니다.");
                }
                
                elapsedTime = Mathf.Clamp(next, 0.0f, duration);
            }
        }
        
        public TValue RemainingTime 
        { 
            get => duration - elapsedTime;
            set
            {
                var next = value;

                if (!IsRunning && !IsPaused)
                {
                    throw new InvalidOperationException("RemainingTime은 Run 또는 Pause 상태일 때만 변경할 수 있습니다.");
                }

                if (next < 0.0f)
                {
                    throw new InvalidTimeException("RemainingTime은 0 이상이어야 합니다.");
                }
                
                elapsedTime = Mathf.Clamp(duration - next, 0.0f, duration);
            }
        }

        public TValue ElapsedTime01  => ElapsedTime / duration;
        public TValue RemainingTime01 => RemainingTime / duration;

    #region 상태

        [SerializeField]
        private TimerState current = TimerState.Ready;
        public TimerState Current
        {
            get => current;
            protected set
            {
                var (prev, next) = (this.current, value);

                // 상태 변화가 없으면 종료합니다.
                if (prev == next) return;

                this.current = next;

                if (invokeEvent.Value)
                {
                    OnStateChange?.Invoke(this, new() { Previous = prev, Current = next });
                }
            }
        }
        
        public bool IsRunning => current == TimerState.Run;
        public bool IsPaused => current == TimerState.Pause;

    #endregion
        
    #region 이벤트

        public event TimerEndEvent<Timer> OnEnd = null;
        public event ValueChangeEvent<Timer, TimerState> OnStateChange = null;
        
        event TimerEndEvent<ITimer> ITimerEventHandler<ITimer>.OnEnd 
        { add => OnEnd += value; remove => OnEnd -= value; }
        event TimerEndEvent<IReadOnlyTimer> ITimerEventHandler<IReadOnlyTimer>.OnEnd
        { add => OnEnd += value; remove => OnEnd -= value; }

        event ValueChangeEvent<ITimer, TimerState> ITimerEventHandler<ITimer>.OnStateChange 
        { add => OnStateChange += value; remove => OnStateChange -= value; }
        event ValueChangeEvent<IReadOnlyTimer, TimerState> ITimerEventHandler<IReadOnlyTimer>.OnStateChange 
        { add => OnStateChange += value; remove => OnStateChange -= value; }

    #endregion

    #region 업데이트

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update(TValue deltaTime)
        {
            if (IsRunning)
            {
                elapsedTime = Mathf.Clamp(elapsedTime + deltaTime, 0.0f, duration);
                
                // 타이머의 시간이 목표 시간을 초과하면 타이머를 종료합니다.
                if (elapsedTime >= duration)
                {
                    if (invokeEvent.Value)
                    {
                        OnEnd?.Invoke(this, new() { });
                    }

                    Stop();
                }
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update() => Update(Time.deltaTime);

    #endregion

    #region 시작, 중지, 일시정지, 재개

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 시작합니다.
        /// </summary>
        /// <param name="duration">지속 시간</param>
        // ------------------------------------------------------------
        public void Start(TValue duration)
        {
            if (IsRunning || IsPaused)
            {
                throw new AlreadyRunningException();
            }
            
            Current = TimerState.Run;

            try
            {
                (Duration, ElapsedTime) = (duration, default);

                cachedStartDuration = duration;
            }
            catch (Exception e)
            {
                Current = TimerState.Ready;
                
                throw e;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 중지합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Stop()
        {
            if (IsRunning || IsPaused)
            {
                Current = TimerState.Ready;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 일시정지합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Pause()
        {
            if (IsRunning)
            {
                Current = TimerState.Pause;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 재개합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Resume()
        {
            if (IsPaused)
            {
                Current = TimerState.Run;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 리셋합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Reset()
        {
            if (IsRunning || IsPaused)
            {
                throw new FailedToResetException();
            }

            (duration, elapsedTime) = (default, default);
        }

    #endregion

    }
}