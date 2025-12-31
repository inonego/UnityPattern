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

    #region 필드

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

    #endregion

    #region 메서드

        public bool IsAcquired(T item)
        {
            return acquired.Contains(item);
        }

        public bool IsReleased(T item)
        {
            // TODO - O(N)에 대해서 최적화 필요
            return released.Contains(item);
        }

        public virtual T Acquire()
        {
            return AcquireInternal();
        }

        public virtual async Awaitable<T> AcquireAsync()
        {
            return await AcquireInternalAsync();
        }

        public virtual void Release(T item, bool pushToReleased = true)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsAcquired(item))
            {
                throw new Exception($"풀에 존재하지 않는 아이템 '{item}'을 제거하려고 했습니다.");
            }

            ReleaseInternal(item, pushToReleased: pushToReleased);
        }
        
        public void ReleaseAll(bool pushToReleased = true)
        {
            foreach (var item in acquired)
            {
                ReleaseInternal(item, removeFromAcquired: false, pushToReleased: pushToReleased);
            }

            acquired.Clear();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// Acquired된 아이템을 다른 풀의 Acquired로 이동합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void MoveAcquiredOneTo(IPool<T> other, T item)
        {
            if (other == null || item == null)
            {
                throw new ArgumentNullException();
            }

            if (!IsAcquired(item))
            {
                throw new Exception($"풀에 존재하지 않는 아이템을 다른 풀로 이동하려고 했습니다.");
            }

            // 제거
            ReleaseInternal(item, pushToReleased: false);

            // 추가
            other.AcquireInternal(item);
        }

        // ----------------------------------------------------------------------
        /// <summary>
        /// <br/>Released된 아이템을 다른 풀의 Released로 이동합니다.
        /// <br/>풀에 남아있는 오브젝트가 없으면 새로운 오브젝트를 생성하여 이동합니다.
        /// </summary>
        // ----------------------------------------------------------------------
        public virtual void MoveReleasedOneTo(IPool<T> other)
        {
            if (other == null)
            {
                throw new ArgumentNullException();
            }

            // 제거
            var item = PopFromReleased();
            
            // 추가
            other.PushToReleased(item);
        }
        
    #endregion

    #region 풀 관리 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 오브젝트를 풀에 추가합니다.
        /// </summary>
        // ------------------------------------------------------------
        public virtual void PushToReleased(T item)
        {
            if (item == null)
            {
                throw new ArgumentNullException();
            }

            if (IsAcquired(item))
            {
                throw new Exception($"사용중인 아이템을 풀에 추가하려고 했습니다.");
            }
            
            // TODO - O(N)에 대해서 최적화 필요
            if (IsReleased(item))
            {
                throw new Exception($"풀에 이미 존재하는 아이템을 추가하려고 했습니다.");
            }

            ReleaseInternal(item, removeFromAcquired: false);
        }

        // -------------------------------------------------------------------------
        /// <summary>
        /// <br/>풀에 남아있는 오브젝트를 풀에서 제거하고 반환합니다.
        /// <br/>풀에 남아있는 오브젝트가 없으면 새로운 오브젝트를 생성하여 반환합니다.
        /// </summary>
        // -------------------------------------------------------------------------
        public virtual T PopFromReleased()
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
                item = AcquireNew();
            }

            return item;
        }
        
        // -------------------------------------------------------------------------
        /// <summary>
        /// <br/>풀에 남아있는 오브젝트를 풀에서 제거하고 반환합니다.
        /// <br/>풀에 남아있는 오브젝트가 없으면 새로운 오브젝트를 생성하여 반환합니다.
        /// </summary>
        // -------------------------------------------------------------------------
        public virtual async Awaitable<T> PopFromReleasedAsync()
        {
            T item;

            // 풀에 남아있는 오브젝트가 있는 경우
            if (released.Count > 0)
            {
                // TODO - 비동기 처리 시 Lock 처리 필요
                item = released.Dequeue();
            }
            else
            {
                // 새로운 오브젝트를 비동기로 생성합니다.
                item = await AcquireNewAsync();
            }

            return item;
        }

    #endregion

    #region 내부 구현용 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T AcquireNew();

        // ------------------------------------------------------------
        /// <summary>
        /// 새로운 오브젝트를 비동기로 생성합니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract Awaitable<T> AcquireNewAsync();

        protected T AcquireInternal()
        {
            T item = PopFromReleased();
            AcquireInternal(item);
            return item;
        }

        protected async Awaitable<T> AcquireInternalAsync()
        {
            T item = await PopFromReleasedAsync();
            AcquireInternal(item);
            return item;
        }

        protected virtual void AcquireInternal(T item)
        {
            acquired.Add(item);
        }

        protected virtual void ReleaseInternal(T item, bool removeFromAcquired = true, bool pushToReleased = true)
        {
            if (removeFromAcquired)
            {
                // 사용중인 오브젝트 목록에서 제거합니다.
                acquired.Remove(item);
            }

            if (pushToReleased)
            {
                // 풀에 남아있는 오브젝트 목록에 추가합니다.
                released.Enqueue(item);
            }
        }

    #endregion

    #region IPool<T> 구현

        void IPool<T>.AcquireInternal(T item)
        {
            AcquireInternal(item);
        }

    #endregion

    }
}