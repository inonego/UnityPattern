using System;

using TValue = System.Single;

namespace inonego
{

    // ==================================================================
    /// <summary>
    /// 타이머 인터페이스입니다.
    /// </summary>
    // ==================================================================
    public interface ITimer
    {

    #region 필드

        public bool IsRunning { get; }
        public bool IsPaused { get; }
    
        public TimerState Current { get; }

    #endregion

    #region 이벤트

        public event EventHandler<TimerEndEventArgs> OnEnd;
        public event ValueChangeEventHandler<TimerState> OnStateChange;

    #endregion

    #region 시간

        public TValue Duration { get; set; }

        // ---------------------------------------------------------------------
        /// <summary>
        /// <br/>타이머가 경과한 시간입니다.
        /// <br/>중지되더라도 <see cref="Reset"/>을 통해 리셋하거나,
        /// <br/><see cref="Start"/>을 통해 새로 시작하기 전까지 값이 유지됩니다.
        /// </summary>
        // ---------------------------------------------------------------------
        public TValue ElapsedTime { get; set; }

        // ---------------------------------------------------------------------
        /// <summary>
        /// <br/>타이머의 남은 시간입니다.
        /// <br/>중지되더라도 <see cref="Reset"/>을 통해 리셋하거나,
        /// <br/><see cref="Start"/>을 통해 새로 시작하기 전까지 값이 유지됩니다.
        /// </summary>
        // ---------------------------------------------------------------------
        public TValue RemainingTime { get; set; }
        
        public TValue ElapsedTime01 { get; }
        public TValue RemainingTime01 { get; }

    #endregion

    #region 메서드
    
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update(TValue deltaTime);
        
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 시작합니다.
        /// </summary>
        /// <param name="duration">지속 시간</param>
        // ------------------------------------------------------------
        public void Start(TValue duration);

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 중지합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Stop();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 일시정지합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Pause();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 재개합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Resume();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 리셋합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Reset();

    #endregion

    }
}