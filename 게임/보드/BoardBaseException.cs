using System;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// 
    /// </summary>
    // ==================================================================
    public abstract partial class BoardBase
    {
        // ------------------------------------------------------------
        /// <summary>
        /// BoardBase 관련 예외의 기본 클래스입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Serializable]
        public class Exception : System.Exception
        {
            public Exception() : base() { }
            public Exception(string message) : base(message) { }
            public Exception(string message, System.Exception innerException) : base(message, innerException) { }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 이미 해당 위치에 공간이 존재할 때 발생하는 예외입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Serializable]
        public class SpaceAlreadyExistsException : Exception
        {
            public SpaceAlreadyExistsException() : base("이미 해당 위치에 공간이 존재합니다.") { }
            public SpaceAlreadyExistsException(string message) : base(message) { }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 해당 위치에 공간이 존재하지 않을 때 발생하는 예외입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Serializable]
        public class SpaceNotFoundException : Exception
        {
            public SpaceNotFoundException() : base("해당 위치에 공간이 존재하지 않습니다.") { }
            public SpaceNotFoundException(string message) : base(message) { }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 유효하지 않은 배치 작업일 때 발생하는 예외입니다.
        /// </summary>
        // ------------------------------------------------------------
        [Serializable]
        public class InvalidPlacementException : Exception
        {
            public InvalidPlacementException() : base("해당 위치에 배치할 수 없습니다.") { }
            public InvalidPlacementException(string message) : base(message) { }
        }
    }
}


