namespace inonego
{
    // ========================================================================
    /// <summary>
    /// 연결해야 하는 객체를 위한 인터페이스입니다.
    /// </summary>
    // ========================================================================
    public interface IConnectable<T>
    {
        public void Connect(T lTarget);
        public void Disconnect();
    }
}