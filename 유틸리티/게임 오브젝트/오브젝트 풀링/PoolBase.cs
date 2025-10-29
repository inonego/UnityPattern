using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Pool
{
    // ============================================================
    /// <summary>
    /// 오브젝트 풀링을 위한 추상 클래스입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public abstract class PoolBase<T> : IPool<T> where T : class
    {
        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 남아있는 오브젝트 목록입니다.
        /// </summary>
        // ------------------------------------------------------------        
        protected Queue<T> released = new();
        public IReadOnlyCollection<T> Released => released;
        
        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 사용중인 오브젝트 목록입니다.
        /// </summary>  
        // ------------------------------------------------------------
        protected HashSet<T> acquired = new();
        public IReadOnlyCollection<T> Acquired => acquired;
        
        protected virtual void OnAcquire(T item) {}
        protected virtual void OnRelease(T item) {}

    #region 메서드

        public T Acquire()
        {
            return AcquireInternal(AcquireNew);
        }

        public void Release(T item)
        {
            ReleaseInternal(item);
        }
        
        public void ReleaseAll()
        {
            foreach (var item in acquired)
            {
                ReleaseInternal(item, removeFromAcquired: false);
            }

            acquired.Clear();
        }

    #endregion

    #region 내부 구현용 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T AcquireNew();

        protected T AcquireInternal(Func<T> acquireNewFunc)
        {
            T item;

            // 풀에 남아있는 오브젝트가 있는 경우
            if (released.Count > 0)
            {
                item = released.Dequeue();
            }
            // 풀에 남아있는 오브젝트가 없는 경우
            else
            {
                // 새로운 오브젝트를 생성합니다.
                item = acquireNewFunc();
            }

            // 사용중인 오브젝트 목록에 추가합니다.
            acquired.Add(item);

            OnAcquire(item);

            return item;
        }

        protected async Awaitable<T> AcquireInternalAsync(Func<Awaitable<T>> acquireNewAsyncFunc)
        {
            T item;

            // 풀에 남아있는 오브젝트가 있는 경우
            if (released.Count > 0)
            {
                item = released.Dequeue();
            }
            // 풀에 남아있는 오브젝트가 없는 경우
            else
            {
                // 새로운 오브젝트를 비동기로 생성합니다.
                item = await acquireNewAsyncFunc();
            }

            // 사용중인 오브젝트 목록에 추가합니다.
            acquired.Add(item);

            OnAcquire(item);

            return item;
        }

        protected void ReleaseInternal(T item, bool removeFromAcquired = true)
        {
            if (!acquired.Contains(item))
            {
                throw new Exception($"풀에 존재하지 않는 아이템 '{item}'을 반환하려고 했습니다.");
            }

            if (removeFromAcquired)
            {
                // 사용중인 오브젝트 목록에서 제거합니다.
                acquired.Remove(item);
            }

            // 풀에 남아있는 오브젝트 목록에 추가합니다.
            released.Enqueue(item);

            OnRelease(item);
        }

    #endregion

    }
}