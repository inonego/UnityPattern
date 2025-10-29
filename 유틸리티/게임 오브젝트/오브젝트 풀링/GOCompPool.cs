using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Pool
{
    // ===============================================================================
    /// <summary>
    /// <br/>유니티에서 사용하는 컴포넌트에 대하여 오브젝트 풀링을 할 수 있습니다.
    /// <br/>컴포넌트를 풀링하기 위해서는 해당 T 컴포넌트가 최상단에 포함된 프리팹이 필요합니다.
    /// </summary>
    // ===============================================================================
    [Serializable]
    public class GOCompPool<T> : GOPool, IPool<T> where T : Component
    {

    #region 필드

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 남아있는 컴포넌트 목록입니다.
        /// </summary>
        // ------------------------------------------------------------
        private Dictionary<GameObject, T> releasedComp = new();
        public IReadOnlyCollection<T> ReleasedComp => releasedComp.Values;

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 사용중인 컴포넌트 목록입니다.
        /// </summary>
        // ------------------------------------------------------------
        private Dictionary<GameObject, T> acquiredComp = new();
        public IReadOnlyCollection<T> AcquiredComp => acquiredComp.Values;

    #endregion

    #region 생성자

        public GOCompPool() : base() {}

        public GOCompPool(IGameObjectProvider provider) : base(provider) {}

    #endregion

    #region GOPool 오버라이드

        protected override void OnAcquire(GameObject gameObject)
        {
            base.OnAcquire(gameObject);

            if (releasedComp.TryGetValue(gameObject, out var comp))
            {
                releasedComp.Remove(gameObject);
            }
            else
            {
                comp = gameObject.GetComponent<T>();
            }   

            if (comp != null)
            {
                acquiredComp.Add(gameObject, comp); 
            } 
        }

        protected override void OnRelease(GameObject gameObject)
        {
            base.OnRelease(gameObject);

            if (acquiredComp.TryGetValue(gameObject, out var comp))
            {
                acquiredComp.Remove(gameObject);
            }
            else
            {
                comp = gameObject.GetComponent<T>();
            }

            if (comp != null)
            {
                releasedComp.Add(gameObject, comp);
            }
        }

    #endregion

    #region 인터페이스 IPool<T> 구현

        IReadOnlyCollection<T> IPool<T>.Released => ReleasedComp;
        IReadOnlyCollection<T> IPool<T>.Acquired => AcquiredComp;

        T IPool<T>.Acquire()
        {
            return AcquireComp();
        }

        void IPool<T>.Release(T comp)
        {
            ReleaseComp(comp);
        }

    #endregion

    #region 메서드

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에서 컴포넌트를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public T AcquireComp()
        {
            var gameObject = Acquire();

            if (acquiredComp.TryGetValue(gameObject, out var comp))
            {
                return comp;
            }

            throw new Exception($"GameObject '{gameObject.name}'에서 컴포넌트 '{typeof(T).Name}'을(를) 찾을 수 없습니다.");
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에서 컴포넌트를 비동기로 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public async Awaitable<T> AcquireCompAsync()
        {
            var gameObject = await AcquireAsync();

            if (acquiredComp.TryGetValue(gameObject, out var comp))
            {
                return comp;
            }

            throw new Exception($"GameObject '{gameObject.name}'에서 컴포넌트 '{typeof(T).Name}'을(를) 찾을 수 없습니다.");
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 풀에 컴포넌트를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public void ReleaseComp(T comp)
        {
            if (comp == null)
            {
                throw new Exception($"컴포넌트 '{typeof(T).Name}'이(가) null입니다.");
            }

            Release(comp.gameObject);
        }

    #endregion

    }
}