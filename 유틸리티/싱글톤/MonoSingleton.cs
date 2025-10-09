using UnityEngine;

namespace inonego
{
    // ==================================================================
    /// <summary>
    /// 기본 MonoBehaviour의 싱글톤을 구현하기 위한 추상 클래스입니다.
    /// </summary>
    // ==================================================================
    public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
    {

    #region 필드

        private static T instance;

        // ------------------------------------------------------------
        /// <summary>
        /// 싱글톤 인스턴스를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindAnyObjectByType<T>();

                    if (instance == null)
                    {
                        instance = new GameObject(typeof(T).Name).AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        public static void ClearInstance() => instance = null;

    #endregion

    #region 유니티 이벤트

        private void Awake()
        {
            if (instance == null)
            {
                instance = this as T;
            }
            else
            {
                #if UNITY_EDITOR
                    Debug.LogWarning($"컴포넌트 '{typeof(T).Name}'의 싱글톤 인스턴스가 이미 존재합니다. 이 게임 오브젝트는 삭제됩니다.");
                #endif

                if (Application.isPlaying)
                {
                    Destroy(gameObject);
                }
                else
                {
                    DestroyImmediate(gameObject);
                }
            }
        }

    #endregion

    }
}