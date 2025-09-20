using System;

using UnityEngine;

using TValue = System.Single;

namespace inonego
{
    public enum TimerState { Ready, Work, Pause }

    public delegate void TimerEndEvent<in TSender>(TSender sender, TimerEndEventArgs e);

    [Serializable]
    public struct TimerEndEventArgs
    {
        // NONE
    }

    public interface ITimerEventHandler<out TSelf>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool InvokeEvent { get; set; }

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
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        [SerializeField]
        private bool invokeEvent = true;
        public bool InvokeEvent
        {
            get => invokeEvent;
            set => invokeEvent = value;
        }

        [SerializeField] private TValue duration = default;
        [SerializeField] private TValue elapsedTime = default;

        public TValue Duration => duration;

        public TValue ElapsedTime => Mathf.Min(duration, elapsedTime);
        public TValue RemainingTime => Mathf.Max(duration - elapsedTime, 0.0f);

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

                if (InvokeEvent)
                {
                    OnStateChange?.Invoke(this, new ValueChangeEventArgs<TimerState> { Previous = prev, Current = next });
                }
            }
        }
        
        public bool IsWorking => current == TimerState.Work;
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
            if (IsWorking)
            {
                if (deltaTime < 0.0f)
                {
                    throw new InvalidDeltaTimeException();
                }

                elapsedTime += deltaTime;
                
                // 타이머의 시간이 목표 시간을 초과하면 타이머를 종료합니다.
                if (elapsedTime >= duration)
                {
                    if (InvokeEvent)
                    {
                        OnEnd?.Invoke(this, new TimerEndEventArgs { });
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
            if (IsWorking || IsPaused)
            {
                throw new AlreadyRunningException();
            }

            if (duration < 0.0f)
            {
                throw new InvalidDurationException();
            }

            Current = TimerState.Work;

            (this.duration, elapsedTime) = (duration, default);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 중지합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Stop()
        {
            if (IsWorking || IsPaused)
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
            if (IsWorking)
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
                Current = TimerState.Work;
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 리셋합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Reset()
        {
            if (IsWorking || IsPaused)
            {
                throw new FailedToResetException();
            }

            (this.duration, elapsedTime) = (default, default);
        }

    #endregion

    }
}