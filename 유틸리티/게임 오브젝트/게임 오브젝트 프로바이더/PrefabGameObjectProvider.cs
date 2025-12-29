using System;

using UnityEngine;

namespace inonego
{
   // ========================================================================
   /// <summary>
   /// 프리팹을 이용하여 게임 오브젝트를 생성하는 기본적인 프로바이더입니다.
   /// </summary>
   // ========================================================================
   [Serializable]
   public class PrefabGameObjectProvider : IGameObjectProvider
   {

   #region 필드

      // ------------------------------------------------------------
      /// <summary>
      /// 프리팹을 이용하여 게임 오브젝트를 생성합니다.
      /// </summary>
      // ------------------------------------------------------------
      public GameObject Prefab
      {
         get => prefab;
         set => prefab = value;
      }

      [SerializeField]
      private GameObject prefab = null;

      // ------------------------------------------------------------
      /// <summary>
      /// <br/>게임 오브젝트를 생성할 위치입니다.
      /// <br/>null인 경우, 루트에 생성됩니다.
      /// </summary>
      // ------------------------------------------------------------
      public Transform Parent
      {
         get => parent;
         set => parent = value;
      }

      [SerializeField]
      private Transform parent = null;

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 생성할 위치를 유지할지에 대한 여부입니다.
      /// </summary>
      // ------------------------------------------------------------
      public bool WorldPositionStays
      {
         get => worldPositionStays;
         set => worldPositionStays = value;
      }

      [SerializeField]
      private bool worldPositionStays = false;

   #endregion

   #region 생성자

      public PrefabGameObjectProvider() : base() {}

      public PrefabGameObjectProvider(GameObject prefab, Transform parent, bool worldPositionStays = true) : this()
      {
         this.prefab = prefab;
         this.parent = parent;

         this.worldPositionStays = worldPositionStays;
      }

   #endregion

   #region 메서드

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 가져옵니다.
      /// </summary>
      // ------------------------------------------------------------
      public GameObject Acquire()
      {
         if (prefab == null)
         {
            throw new NullReferenceException("프리팹이 설정되지 않았습니다.");
         }

         var instance = GameObject.Instantiate(prefab, parent, worldPositionStays);
         
         return instance;
      }

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 반환합니다.
      /// </summary>
      // ------------------------------------------------------------
      public void Release(GameObject go)
      {
         if (go != null)
         {
            GameObject.Destroy(go);
         }
      }

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 비동기로 가져옵니다.
      /// </summary>
      // ------------------------------------------------------------
      public async Awaitable<GameObject> AcquireAsync()
      {
         if (prefab == null)
         {
            throw new NullReferenceException("프리팹이 설정되지 않았습니다.");
         }

         var instances = await GameObject.InstantiateAsync(prefab);
         
         if (instances != null && instances.Length > 0)
         {
            var instance = instances[0];

            if (parent != null)
            {
               instance.transform.SetParent(parent, worldPositionStays);
            }

            return instance;
         }
         
         return null;
      }

   #endregion

   }
}