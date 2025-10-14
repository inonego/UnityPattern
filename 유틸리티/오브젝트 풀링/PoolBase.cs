using System;
using System.Collections;
using System.Collections.Generic;

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
        private Queue<T> released = new Queue<T>();
        public IReadOnlyCollection<T> Released => released;
        
        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 사용중인 오브젝트 목록입니다.
        /// </summary>  
        // ------------------------------------------------------------
        private List<T> acquired = new List<T>();
        public IReadOnlyCollection<T> Acquired => acquired;
        
        protected virtual void OnAcquire(T item) {}
        protected virtual void OnRelease(T item) {}

        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T Create();

        public T Acquire()
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
                item = Create();
            }

            // 사용중인 오브젝트 목록에 추가합니다.
            acquired.Add(item);

            OnAcquire(item);

            return item;
        }

        private void _Release(T item)
        {
            OnRelease(item);

            // 사용중인 오브젝트 목록에서 제거합니다.
            acquired.Remove(item);

            // 풀에 남아있는 오브젝트 목록에 추가합니다.
            released.Enqueue(item);
        }

        private void _Release(int index)
        {
            var item = acquired[index];

            OnRelease(item);

            // 사용중인 오브젝트 목록에서 제거합니다.
            acquired.RemoveAt(index);

            // 풀에 남아있는 오브젝트 목록에 추가합니다.
            released.Enqueue(item);
        }

        public void Release(T item)
        {
            if (acquired.Contains(item))
            {
                _Release(item);
            }
            else
            {
                throw new Exception($"풀에 존재하지 않는 아이템 '{item}'을 반환하려고 했습니다.");
            }
        }

        public void ReleaseAll()
        {
            for (int i = acquired.Count - 1; i >= 0; i--)
            {
                _Release(i);
            }
        }
    }
}