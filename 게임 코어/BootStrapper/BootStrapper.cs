using System;

using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace inonego.Core.Bootstrapper
{
    public class Bootstrapper : MonoBehaviour
    {

    #if UNITY_EDITOR
        private static int initSceneIndex = -1;
        private static string initScenePath = null;
    #endif

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitBootstrapper()
        {
            var settings = BootstrapperSettings.Instance;

            if (!settings.BootstrapperSceneIndex.HasValue) return;
            int bootstrapperSceneIndex = settings.BootstrapperSceneIndex.Value;

            var activeScene = SceneManager.GetActiveScene();

        #if UNITY_EDITOR
            initSceneIndex = activeScene.buildIndex;
            initScenePath = activeScene.path;
        #endif

            if (activeScene.buildIndex != bootstrapperSceneIndex)
            {
                // 부트스트래퍼 씬으로 강제 이동
                SceneManager.LoadScene(bootstrapperSceneIndex);
            }

            var go = new GameObject("Bootstrapper");
            var bootstrapper = go.AddComponent<Bootstrapper>();
        }

        private async void Awake()
        {
            await InitModules();

            await Awaitable.NextFrameAsync();
            
            LoadInitScene();
        }

        // -------------------------------------------------------------
        /// <summary>
        /// 부트스트래퍼 모듈을 초기화합니다.
        /// </summary>
        // -------------------------------------------------------------
        private async Awaitable InitModules()
        {
            var settings = BootstrapperSettings.Instance;
            if (settings.ModuleScripts == null || settings.ModuleScripts.Count == 0) return;

            foreach (var moduleScript in settings.ModuleScripts)
            {
                Type type = moduleScript.Type;
                if (type == null) continue;
                
                var module = Activator.CreateInstance(type) as IBootstrapperModule;
                
                if (module == null)
                {
                    throw new Exception($"[Bootstrapper] {type.Name} is not IBootstrapperModule");
                }
                
                await module.Init();
            }
        }

        // -------------------------------------------------------------
        /// <summary>
        /// 부트스트래퍼 실행이 완료된 후 초기 씬을 로드합니다.
        /// </summary>
        // -------------------------------------------------------------
        private void LoadInitScene()
        {
            if (Application.isPlaying)
            {
                var activeScene = SceneManager.GetActiveScene();
                    
            #if UNITY_EDITOR

                if (!string.IsNullOrEmpty(initScenePath))
                {
                    if (activeScene.buildIndex != initSceneIndex)
                    {
                        var param = new LoadSceneParameters(LoadSceneMode.Single);
                        EditorSceneManager.LoadSceneInPlayMode(initScenePath, param);
                    }

                    // 씬 경로 초기화
                    initSceneIndex = -1;
                    initScenePath = null;

                    return;
                }

            #endif

                var settings = BootstrapperSettings.Instance;

                if (settings.SceneIndexToLoad.HasValue)
                {
                    int initSceneIndex = settings.SceneIndexToLoad.Value;

                    if (activeScene.buildIndex != initSceneIndex)
                    {
                        SceneManager.LoadScene(initSceneIndex);
                    }
                }
            }
        }
    }
}
