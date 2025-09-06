using System;

using TValue = System.Double;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// <br/>읽기 전용 타이머 인터페이스입니다.
    /// <br/>타이머의 상태를 읽기 전용으로 제공합니다.
    /// </summary>
    // ==================================================================
    public interface ITimer
    {        
        public TValue Duration { get; }

        public TValue ElapsedTime { get; }
        public TValue RemainingTime { get; }
        
        public TValue ElapsedTime01 { get; }
        public TValue RemainingTime01 { get; }

        public bool IsWorking { get; }
        public bool IsPaused { get; }
    
        public enum State { Begin, Pause, End }

        public State Current { get; }

    #region 이벤트

        [Serializable]
        public struct EndEventArgs
        {
            // NONE
        }

        public event ValueChangeEvent<ITimer, State> OnStateChange;
        public event Action<ITimer, EndEventArgs> OnEnd;

    #endregion

    }
}