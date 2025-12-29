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
    public class GOCompPool<T> : PoolBase<T>, IGOCompPool where T : Component
    {

    #region 필드

        [SerializeReference]
        protected IGameObjectProvider gameObjectProvider = new PrefabGameObjectProvider();
        public IGameObjectProvider GameObjectProvider => gameObjectProvider;

        [SerializeField]
        protected Transform pool = null;
        public Transform Pool
        {
            get => pool;
            set => pool = value;
        }

        public Transform Parent 
        { 
            get => gameObjectProvider.Parent;
            set => gameObjectProvider.Parent = value;
        }

        public bool WorldPositionStays
        { 
            get => gameObjectProvider.WorldPositionStays;
            set => gameObjectProvider.WorldPositionStays = value;
        }

    #endregion

    #region 생성자

        public GOCompPool() : base() {}

        public GOCompPool(IGameObjectProvider gameObjectProvider) : base() 
        {
            if (gameObjectProvider == null)
            {
                throw new ArgumentNullException("게임 오브젝트 프로바이더가 null입니다.");
            }

            this.gameObjectProvider = gameObjectProvider;
        }

    #endregion

    #region PoolBase 오버라이드

        protected override T AcquireNew()
        {
            if (GameObjectProvider == null)
            {
                throw new NullReferenceException("GameObjectProvider가 설정되지 않았습니다.");
            }

            var gameObject = GameObjectProvider.Acquire();
            var comp = gameObject.GetComponent<T>();

            if (comp == null)
            {
                throw new Exception($"게임 오브젝트 '{gameObject.name}'에서 컴포넌트 '{typeof(T).Name}'을(를) 찾을 수 없습니다.");
            }

            return comp;
        }

        protected async Awaitable<T> AcquireNewAsync()
        {
            if (GameObjectProvider == null)
            {
                throw new NullReferenceException("GameObjectProvider가 설정되지 않았습니다.");
            }

            var gameObject = await GameObjectProvider.AcquireAsync();
            var comp = gameObject.GetComponent<T>();
            
            if (comp == null)
            {
                throw new Exception($"게임 오브젝트 '{gameObject.name}'에서 컴포넌트 '{typeof(T).Name}'을(를) 찾을 수 없습니다.");
            }
            
            return comp;
        }
        
        // ------------------------------------------------------------
        /// <summary>
        /// 풀에서 컴포넌트를 비동기로 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public async Awaitable<T> AcquireAsync()
        {
            return await AcquireInternalAsync(AcquireNewAsync);
        }

        protected override void OnAcquire(T item)
        {
            var gameObject = item.gameObject;

            if (gameObject.transform.parent != Parent)
            {
                gameObject.transform.SetParent(Parent, WorldPositionStays);
            }

            gameObject.SetActive(true);
        }

        protected override void OnRelease(T item)
        {
            var gameObject = item.gameObject;

            if (gameObject.transform.parent != Pool)
            {
                gameObject.transform.SetParent(Pool, WorldPositionStays);
            }

            gameObject.SetActive(false);
        }

    #endregion

    #region IGameObjectProvider 구현

        GameObject IGameObjectProvider.Acquire()
        {
            var comp = Acquire();

            return comp.gameObject;
        }

        async Awaitable<GameObject> IGameObjectProvider.AcquireAsync()
        {
            var comp = await AcquireAsync();

            return comp.gameObject;
        }

        void IGameObjectProvider.Release(GameObject go)
        {
            if (go == null)
            {
                throw new ArgumentNullException("타겟이 null입니다.");
            }
            
            if (go.TryGetComponent(out T comp))
            {
                Release(comp);
            }
        }

    #endregion

    }
}
