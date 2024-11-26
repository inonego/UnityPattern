using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace inonego
{

[Serializable]
    public class Pool
    {
        public Transform Parent { get; private set; }

        public GameObject Prefab;

        private Queue<GameObject> left = new Queue<GameObject>();
        public IReadOnlyCollection<GameObject> Left => left;
        
        private List<GameObject> spawned = new List<GameObject>();
        public IReadOnlyList<GameObject> Spawned => spawned;
        
        public int InitalCount = 0;
        
        public int LeftCount    => left.Count;
        public int SpawnedCount => spawned.Count;
        
        public int TotalCount   => LeftCount + SpawnedCount;

        public void Init(Transform parent)
        {
            this.Parent = parent;
            
            // 초기 개수만큼 게임 오브젝트를 추가합니다.
            for (int i = 0; i < InitalCount; i++)
            {
                left.Enqueue(InstantiateGO());
            }
        }

        private void AttachToParent(GameObject GO)
        {
            GO.transform.SetParent(Parent);
            
            // 위치 및 회전 설정
            GO.transform.localPosition = Vector3.zero;
            GO.transform.localRotation = Quaternion.identity;
        }

        private void DetachFromParent(GameObject GO, Vector3 position, Quaternion rotation)
        {
            GO.transform.SetParent(null);
            
            // 위치 및 회전 설정
            GO.transform.position = position;
            GO.transform.rotation = rotation;
        }
        
        private GameObject InstantiateGO()
        {
            GameObject GO = GameObject.Instantiate(Prefab);
            
            // 게임 오브젝트 상태 설정
            GO.SetActive(false);
            AttachToParent(GO);

            return GO;
        }

        public GameObject Spawn()
        {
            return Spawn(Vector3.zero, Quaternion.identity);
        }

        public GameObject Spawn(Vector3 position, Quaternion rotation)
        {
            // 풀 목록에서 제거
            if (!left.TryDequeue(out GameObject GO))
            {
                // 부족하면 게임 오브젝트 생성
                GO = InstantiateGO();
            }

            // 게임 오브젝트 상태 설정
            DetachFromParent(GO, position, rotation);
            GO.SetActive(true);
            
            // 스폰 목록에 추가
            spawned.Add(GO);
            PoolUtil.Register(this, GO);
            
            return GO;
        }

        internal void Despawn(GameObject GO)
        {
            if (spawned.Contains(GO))
            {
                // 스폰 목록에서 제거
                spawned.Remove(GO);
                
                // 게임 오브젝트 상태 설정
                GO.SetActive(false);
                AttachToParent(GO);

                // 풀 목록에 추가
                left.Enqueue(GO);
                PoolUtil.Remove(GO);
            }
            #if UNITY_EDITOR
                else
                { 
                    Debug.LogError($"{GO.name}(은)는 {Prefab.name}의 풀에 존재하지 않습니다.");
                }
            #endif
        }
        
        public void DespawnAll()
        {
            List<GameObject> spawned = new List<GameObject>();
            
            spawned.AddRange(this.spawned);

            for (int i = 0; i < spawned.Count; i++)
            {
                Despawn(spawned[i]);
            }
            
            spawned.Clear();
        }
    }

public class PoolPack : MonoBehaviour
{
    
    [SerializeField] private List<Pool> poolList = new List<Pool>();

    public IReadOnlyList<Pool> PoolList => poolList;

    private void Awake()
    {
        foreach (var pool in poolList)
        {
            pool.Init(transform);
        }
    }

    private void OnDestroy()
    {
        DespawnAll();
    }

    public void DespawnAll()
    {
        foreach (var pool in poolList)
        {
            pool.DespawnAll();
        }
    }
}

public static class PoolUtil
{
    private static Dictionary<GameObject, Pool> GOPool = new Dictionary<GameObject, Pool>();

    internal static void Register(Pool pool, GameObject GO)
    {
        GOPool[GO] = pool;
    }

    internal static void Remove(GameObject GO)
    {
        GOPool.Remove(GO);
    }
    
    public static void Despawn(this GameObject GO)
    {
        if (GOPool.ContainsKey(GO))
        {
            GOPool[GO].Despawn(GO);
        }
        else
        {
            GameObject.Destroy(GO);
        }
    }
}

public partial class Health
{
    private partial void Destroy() => gameObject.Despawn();
}

}