namespace inonego
{
    // ==================================================================
    /// <summary>
    /// 싱글톤을 구현하기 위한 추상 클래스입니다.
    /// </summary>
    // ==================================================================
    public abstract class Singleton<T> where T : Singleton<T>, new()
    {

    #region 필드

        private static T instance;

        // ------------------------------------------------------------
        /// <summary>
        /// 싱글톤 인스턴스를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (typeof(T))
                    {
                        if (instance == null)
                        {
                            instance = new();
                        }
                    }
                }

                return instance;
            }
        }

        public static void ClearInstance() => instance = null;

    #endregion

    }

}