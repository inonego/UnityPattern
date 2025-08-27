namespace inonego
{
    // ========================================================================================================
    /// <summary>
    /// <br/>기존의 UnityEngine.Debug 클래스를 대체하여, 로그를 남길 수 있게하는 객체를 생성하기 위한 인터페이스입니다.
    /// <br/>이 인터페이스를 구현하여 로깅하면 무엇이 좋느냐, 의존성 주입을 통해 Logger 객체를 교체함으로써,
    /// <br/>상황에 따른 적절한 로깅 액션(에디터 출력, 파일로 저장 등등)을 취할 수 있습니다.
    /// <br/>
    /// <br/>인스펙터에 출력하고 싶으면 SerializeReference 애트리뷰트를 사용하세요!
    /// <br/>예시) [SerializeReference] private ILogger logger = new UnityDebugLogger();
    /// </summary>
    // ========================================================================================================
    public interface ILogger
    {
        void Log(string message);
        void LogWarning(string message);
        void LogError(string message);
    }
}