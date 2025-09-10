using System;

using TValue = System.Double;

namespace inonego
{
    public enum TimerState { Begin, Pause, End }

    [Serializable]
    public struct TimerEndEventArgs
    {
        // NONE
    }

    public interface ITimerEventHandler<TSelf> where TSelf : ITimerEventHandler<TSelf>
    {
        public event ValueChangeEvent<TSelf, TimerState> OnStateChange;
        public event Action<TSelf, TimerEndEventArgs> OnEnd;
    }

    public interface IReadOnlyTimer : ITimerEventHandler<IReadOnlyTimer>
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 이벤트를 호출할지 여부를 결정합니다.
        /// </summary>
        // ------------------------------------------------------------
        public bool InvokeEvent { get; set; }

    #region 시간

        public TValue Duration { get; }

        public TValue ElapsedTime { get; }
        public TValue RemainingTime { get; }
        
        public TValue ElapsedTime01 { get; }
        public TValue RemainingTime01 { get; }

    #endregion

    #region 상태

        public bool IsWorking { get; }
        public bool IsPaused { get; }
    
        public TimerState Current { get; }

    #endregion

    }
    
    // ==================================================================
    /// <summary>
    /// 타이머 인터페이스입니다.
    /// </summary>
    // ==================================================================
    public interface ITimer : IReadOnlyTimer, ITimerEventHandler<ITimer>
    {

    #region 상태
    
        public enum State { Begin, Pause, End }

    #endregion

    #region 메서드
    
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        void Update(float deltaTime);
        
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        void Update();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 시작합니다.
        /// </summary>
        /// <param name="duration">지속 시간</param>
        // ------------------------------------------------------------
        void Start(TValue duration);

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 중지합니다.
        /// </summary>
        // ------------------------------------------------------------
        void Stop();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 일시정지합니다.
        /// </summary>
        // ------------------------------------------------------------
        void Pause();

        // ------------------------------------------------------------
        /// <summary>
        /// 타이머의 작동을 재개합니다.
        /// </summary>
        // ------------------------------------------------------------
        void Resume();

    #endregion

    }
}