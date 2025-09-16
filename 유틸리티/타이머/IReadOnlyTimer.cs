using TValue = System.Double;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// 읽기 전용 타이머 인터페이스입니다.
    /// </summary>
    // ==================================================================
    public interface IReadOnlyTimer : ITimerEventHandler<IReadOnlyTimer>
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

    }
}