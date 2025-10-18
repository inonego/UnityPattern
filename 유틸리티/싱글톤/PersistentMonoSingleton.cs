namespace inonego
{
    // ==================================================================
    /// <summary>
    /// <br/>기본 MonoBehaviour의 싱글톤을 구현하기 위한 추상 클래스입니다.
    /// <br/>이 클래스를 상속받은 싱글톤 인스턴스는 씬이 변경되어도 유지됩니다.
    /// </summary>
    // ==================================================================
    public abstract class PersistentMonoSingleton<T> : MonoSingleton<T> where T : PersistentMonoSingleton<T>
    {
        protected internal override bool isDontDestroyOnLoad => true;
    }
}