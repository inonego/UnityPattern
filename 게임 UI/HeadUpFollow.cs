using System;

using UnityEngine;

namespace inonego
{
    [RequireComponent(typeof(Billboard))]
    public class HeadUpFollow : MonoBehaviour
    {
        
    #region 필드

        // --------------------------------------------------------
        /// <summary>
        /// 이동시킬 루트 게임 오브젝트의 트랜스폼입니다.
        /// </summary>
        // --------------------------------------------------------
        [Tooltip("이동시킬 루트 게임 오브젝트의 트랜스폼입니다.")]
        [SerializeField]
        private Transform root;
        public Transform Root => root;

        // --------------------------------------------------------
        /// <summary>
        /// 머리 부분을 찾기 위한 기준이 되는 트랜스폼입니다.
        /// </summary>
        // --------------------------------------------------------
        [Tooltip("머리 부분을 찾기 위한 기준이 되는 트랜스폼입니다.\n하위에서 'Head'가 포함된 이름으로 찾습니다.\n모델링의 게임오브젝트를 드래그해서 넣어주세요.")]
        [SerializeField]
        private Transform reference;
        public Transform Reference => reference;

        // --------------------------------------------------------
        /// <summary>
        /// 머리 부분에 대한 트랜스폼입니다.
        /// </summary>
        // --------------------------------------------------------
        [Tooltip("머리 부분에 대한 트랜스폼입니다.\n미리 할당하지 않는 경우 자동으로 검색합니다.")]
        [SerializeField]
        private Transform headTransform;
        public Transform HeadTransform
        {
            get => headTransform;
            set => headTransform = value;
        }

        // --------------------------------------------------------
        /// <summary>
        /// 위치에 대한 오프셋입니다.
        /// </summary>
        // --------------------------------------------------------
        public Vector3 Offset = Vector3.up;

    #endregion

    #region 컴포넌트
    
        private Billboard billboard;

    #endregion

    #region 초기화
        
        private void Awake()
        {
            GetComponents();
            
            if (headTransform == null)
            {
                FindHead();
            }
        }

        private void GetComponents()
        {
            if (billboard == null)
            {
                billboard = GetComponent<Billboard>();
            }
        }

    #endregion

    #region Head 탐색 관련

        public void FindHead()
        {
            // "Head"가 포함된 이름으로 찾기
            headTransform = FindChildContainingName(reference, "Head");
            
            if (headTransform != null) return;

            // 마지막으로 레퍼런스 자체 사용
            headTransform = reference;
        }

        private Transform FindChildContainingName(Transform parent, string name)
        {
            if (parent.name.Contains(name, StringComparison.OrdinalIgnoreCase)) 
            {
                return parent;
            }

            for (int i = 0; i < parent.childCount; i++)
            {
                Transform result = FindChildContainingName(parent.GetChild(i), name);
                
                if (result != null) return result;
            }

            return null;
        }

    #endregion

    #region 업데이트

        private void LateUpdate()
        {
            if (root == null || reference == null || headTransform == null) return;

            root.position = headTransform.position + root.TransformDirection(Offset);
        }

    #endregion

    }
}
