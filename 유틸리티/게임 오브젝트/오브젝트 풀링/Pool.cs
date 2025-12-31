using System;
using System.Threading;
using System.Threading.Tasks;

using UnityEngine;

namespace inonego.Pool
{
    // ===================================================================
    /// <summary>
    /// UnityEngine.Object가 아닌 일반적인 클래스에 사용 가능한 풀입니다.
    /// </summary>
    // ===================================================================
    [Serializable]
    public class Pool<T> : PoolBase<T> where T : class, new()
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override T AcquireNew()
        {
            return new T();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 비동기로 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected override async Awaitable<T> AcquireNewAsync()
        {
            return await Task.FromResult(new T());
        }
    }
}