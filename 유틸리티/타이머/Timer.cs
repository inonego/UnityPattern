using System;

using UnityEngine;

using TValue = System.Double;

namespace inonego
{
    using static ITimer;

    // ==================================================================
    /// <summary>
    /// <br/>타이머 클래스입니다.
    /// <br/>업데이트 메서드를 통해서, 타이머의 작동 시간을 업데이트해야합니다.
    /// </summary>
    // ==================================================================
    [Serializable]
    public class Timer : ITimer
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool InvokeEvent = true;

        [SerializeField] private TValue duration = default;
        [SerializeField] private TValue elapsedTime = default;

        public TValue Duration => duration;

        public TValue ElapsedTime => Math.Min(duration, elapsedTime);
        public TValue RemainingTime => Math.Max(duration - elapsedTime, 0.0);

        public TValue ElapsedTime01  => ElapsedTime / duration;
        public TValue RemainingTime01 => RemainingTime / duration;

    #region 상태

        [SerializeField]
        private State current = State.Begin;
        public State Current
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
                    OnStateChange?.Invoke(this, new ValueChangeEventArgs<State> { Previous = prev, Current = next });
                }
            }
        }
        
        public bool IsWorking => current == State.Begin;
        public bool IsPaused => current == State.Pause;

    #endregion
        
    #region 이벤트

        public event ValueChangeEvent<Timer, State> OnStateChange = null;
        public event Action<Timer, EndEventArgs> OnEnd = null;

        event ValueChangeEvent<ITimer, State> ITimer.OnStateChange
        {
            add => OnStateChange += value;
            remove => OnStateChange -= value;
        }

        event Action<ITimer, EndEventArgs> ITimer.OnEnd
        {
            add => OnEnd += value;
            remove => OnEnd -= value;
        }

    #endregion

    #region 업데이트

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update(float deltaTime)
        {
            if (IsWorking)
            {
                elapsedTime += deltaTime;
                
                // 타이머의 시간이 목표 시간을 초과하면 타이머를 종료합니다.
                if (elapsedTime >= duration)
                {
                    if (InvokeEvent)
                    {
                        OnEnd?.Invoke(this, new EndEventArgs { });
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
                throw new InvalidOperationException("타이머가 이미 작동 중입니다. 중지 후 시작해주세요.");
            }

            Current = State.Begin;

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
                Current = State.End;
                    
                (this.duration, elapsedTime) = (default, default);
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
                Current = State.Pause;
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
                Current = State.Begin;
            }
        }

    #endregion

    }
}