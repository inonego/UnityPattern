using System;

using UnityEngine;

namespace inonego
{
    // ==============================================================================
    /// <summary>
    /// <br/>유니티 에디터 환경에서 로그를 출력하도록 하는 Logger 클래스입니다.
    /// <br/>UNITY_EDITOR 조건부 컴파일을 사용하여, 빌드 시에는 로그가 출력되지 않습니다.
    /// </summary>
    // ==============================================================================
    [Serializable]
    public class UnityDebugLogger : LoggerBase
    {
        public override void Log(string message)
        {
        #if UNITY_EDITOR
            if (log) Debug.Log(message);
        #endif
        }

        public override void LogWarning(string message)
        {
        #if UNITY_EDITOR
            if (log) Debug.LogWarning(message);
        #endif
        }

        public override void LogError(string message)
        {
        #if UNITY_EDITOR
            if (log) Debug.LogError(message);
        #endif
        }
    }
}