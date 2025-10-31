using System;

using TValue = System.Single;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// 읽기 전용 타이머 인터페이스입니다.
    /// </summary>
    // ==================================================================
    public interface IReadOnlyTimer
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

        public TValue Duration { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머가 경과한 시간입니다.
        /// </summary>
        // ------------------------------------------------------------
        public TValue ElapsedTime { get; }

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 남은 시간입니다.
        /// </summary>
        // ------------------------------------------------------------
        public TValue RemainingTime { get; }
        
        public TValue ElapsedTime01 { get; }
        public TValue RemainingTime01 { get; }

    #endregion

    }
}