using System;

using UnityEngine;

namespace inonego.Pool
{
    // ============================================================
    /// <summary>
    /// 게임 오브젝트를 생성하는 오브젝트 풀링을 위한 풀입니다.
    /// </summary>
    // ============================================================
    [Serializable]
    public class GOPool : PoolBase<GameObject>, IGOPool
    {

    #region 필드
    
        [SerializeField]
        protected IGameObjectProvider gameObjectProvider = null;
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

        public GOPool() : this(null) {}

        public GOPool(IGameObjectProvider provider)
        {
            gameObjectProvider = provider != null ? provider : new PrefabGameObjectProvider();
        }

    #endregion

    #region 메서드

        protected override GameObject AcquireNew()
        {
            return GameObjectProvider.Acquire();
        }

        public async Awaitable<GameObject> AcquireAsync()
        {
            GameObject item;

            // 풀에 남아있는 오브젝트가 있는 경우
            if (released.Count > 0)
            {
                item = released.Dequeue();
            }
            // 풀에 남아있는 오브젝트가 없는 경우
            else
            {
                // 새로운 오브젝트를 비동기로 생성합니다.
                item = await GameObjectProvider.AcquireAsync();
            }

            // 사용중인 오브젝트 목록에 추가합니다.
            acquired.Add(item);

            OnAcquire(item);

            return item;
        }

        protected override void OnAcquire(GameObject gameObject)
        {
            if (gameObject.transform.parent != Parent)
            {
                gameObject.transform.SetParent(Parent, WorldPositionStays);
            }

            gameObject.SetActive(true);
        }

        protected override void OnRelease(GameObject gameObject)
        {
            if (gameObject.transform.parent != Pool)
            {
                gameObject.transform.SetParent(Pool, WorldPositionStays);
            }

            gameObject.SetActive(false);
        }
        
    #endregion

    }
}