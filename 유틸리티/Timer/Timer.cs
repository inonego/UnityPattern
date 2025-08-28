using System;

using UnityEngine;

using TValue = System.Double;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// <br/>타이머 클래스입니다.
    /// <br/>업데이트 메서드를 통해서, 타이머의 작동 시간을 업데이트해야합니다.
    /// </summary>
    // ==================================================================
    [Serializable]
    public class Timer
    {
        [SerializeField] private TValue duration = default;
        [SerializeField] private TValue elapsedTime = default;

        public TValue Duration => duration;

        public TValue ElapsedTime => Math.Min(duration, elapsedTime);
        public TValue RemainingTime => Math.Max(duration - elapsedTime, 0.0);

        public TValue ElapsedTime01  => ElapsedTime / duration;
        public TValue RemainingTime01 => RemainingTime / duration;

    #region 열거형 타입 정의

        public enum State { Begin, Pause, End }

    #endregion

    #region 상태

        [SerializeField]
        private State current = State.Begin;
        public State Current => current;

        // 내부적으로 상태를 변경할때 state 프로퍼티를 사용하세요!
        private State state
        {
            get => current;
            set
            {
                var (prev, next) = (current, value);

                // 상태 변화가 없으면 종료합니다.
                if (prev == next) return;

                current = next;

                OnStateChange?.Invoke(this, new ValueChangeEventArgs<State> { Previous = prev, Current = current });
            }
        }
        
        public bool IsWorking => Current == State.Begin;
        public bool IsPaused => Current == State.Pause;

    #endregion
        
    #region 이벤트

        public event ValueChangeEvent<Timer, State> OnStateChange = null;

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

            state = State.Begin;

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
                state = State.End;
                    
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
                state = State.Pause;
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
                state = State.Begin;
            }
        }

    #endregion

    }
}