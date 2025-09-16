using TValue = System.Double;

namespace inonego
{

    // ==================================================================
    /// <summary>
    /// 타이머 인터페이스입니다.
    /// </summary>
    // ==================================================================
    public interface ITimer : ITimerEventHandler<ITimer>
    {

    #region 상태

        public bool IsWorking { get; }
        public bool IsPaused { get; }
    
        public TimerState Current { get; }

    #endregion

    #region 시간

        public TValue Duration { get; }

        public TValue ElapsedTime { get; }
        public TValue RemainingTime { get; }
        
        public TValue ElapsedTime01 { get; }
        public TValue RemainingTime01 { get; }

    #endregion

    #region 메서드
    
        // ------------------------------------------------------------
        /// <summary>
        /// 타이머를 업데이트합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void Update(float deltaTime);
        
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

    #endregion

    }
}