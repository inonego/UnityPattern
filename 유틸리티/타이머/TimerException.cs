using System;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// Timer 클래스 내부의 Exception들을 정의합니다.
    /// </summary>
    // ==================================================================
    public partial class Timer
    {
        // ==================================================================
        /// <summary>
        /// Timer 관련 예외의 기본 클래스입니다.
        /// </summary>
        // ==================================================================
        [Serializable]
        public class Exception : System.Exception
        {
            public Exception() : base() { }
            public Exception(string message) : base(message) { }
            public Exception(string message, System.Exception innerException) : base(message, innerException) { }
        }

        // ==================================================================
        /// <summary>
        /// Timer가 이미 작동 중일 때 발생하는 예외입니다.
        /// </summary>
        // ==================================================================
        [Serializable]
        public class AlreadyRunningException : Exception
        {
            public AlreadyRunningException() : base("타이머가 이미 작동 중입니다. 중지 후 시작해주세요.") { }
            public AlreadyRunningException(string message) : base(message) { }
        }

        // ==========================================================================
        /// <summary>
        /// 타이머가 작동 중이거나 일시정지 중일 때 Reset을 시도할 때 발생하는 예외입니다.
        /// </summary>
        // ==========================================================================
        [Serializable]
        public class FailedToResetException : Exception
        {
            public FailedToResetException() : base("타이머가 작동 중이거나 일시정지 중입니다. 정지 후 리셋해주세요.") { }
            public FailedToResetException(string message) : base(message) { }
        }

        // ==================================================================
        /// <summary>
        /// 유효하지 않은 시간 값을 설정할 때 발생하는 예외입니다.
        /// </summary>
        // ==================================================================
        [Serializable]
        public class InvalidTimeException : Exception
        {
            public InvalidTimeException() : base("시간 값은 0 이상이어야 합니다.") { }
            public InvalidTimeException(string message) : base(message) { }
        }

    }
}
