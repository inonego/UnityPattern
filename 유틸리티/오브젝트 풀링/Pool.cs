using System;

namespace inonego.Pool
{
    // ===================================================================
    /// <summary>
    /// UnityEngine.Object가 아닌 일반적인 클래스에 사용 가능한 풀입니다.
    /// </summary>
    // ===================================================================
    [Serializable]
    public abstract class Pool<T> : PoolBase<T> where T : class, new()
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override T Create()
        {
            return new T();
        }
    }
}