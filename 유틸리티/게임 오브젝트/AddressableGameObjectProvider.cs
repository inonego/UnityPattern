using System;

using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace inonego
{
   // ========================================================================
   /// <summary>
   /// Addressables를 사용하여 게임 오브젝트를 생성하는 프로바이더입니다.
   /// </summary>
   // ========================================================================
   [Serializable]
   public class AddressableGameObjectProvider : IGameObjectProvider
   {

   #region 필드

      // -------------------------------------------------------------------
      /// <summary>
      /// Addressables Asset Reference를 이용하여 게임 오브젝트를 생성합니다.
      /// </summary>
      // -------------------------------------------------------------------
      public AssetReferenceGameObject AssetReference
      {
         get => assetReference;
         set => assetReference = value;
      }

      [SerializeField]
      private AssetReferenceGameObject assetReference = null;

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

   #region 메서드

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 가져옵니다.
      /// </summary>
      // ------------------------------------------------------------
      public GameObject Acquire()
      {
         if (assetReference == null)
         {
            throw new NullReferenceException("AssetReference가 설정되지 않았습니다.");
         }

         AsyncOperationHandle<GameObject> handle = assetReference.InstantiateAsync();

         var instance = handle.WaitForCompletion();

         if (instance != null && parent != null)
         {
            instance.transform.SetParent(parent, worldPositionStays);
         }

         return instance;
      }

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 반환합니다.
      /// </summary>
      // ------------------------------------------------------------
      public void Release(GameObject target)
      {
         if (target != null)
         {
            Addressables.ReleaseInstance(target);
         }
      }

      // ------------------------------------------------------------
      /// <summary>
      /// 게임 오브젝트를 비동기로 가져옵니다.
      /// </summary>
      // ------------------------------------------------------------
      public async Awaitable<GameObject> AcquireAsync()
      {
         if (assetReference == null)
         {
            throw new NullReferenceException("AssetReference가 설정되지 않았습니다.");
         }

         AsyncOperationHandle<GameObject> handle = assetReference.InstantiateAsync();
         
         var instance = await handle.Task;

         if (instance != null && parent != null)
         {
            instance.transform.SetParent(parent, worldPositionStays);
         }

         return instance;
      }

   #endregion

   }
}