#if UNITY_EDITOR

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEngine;
using UnityEngine.UIElements;

using UnityEditor;
using UnityEditor.SceneManagement;

namespace inonego.Core.Bootstrapper.Editor
{
    using Serializable;

    [InitializeOnLoad]
    public static class BuildProfileWindowExtender
    {
        private const string WINDOW_TYPE_NAME = "UnityEditor.Build.Profile.BuildProfileWindow";

        private static string UssPath => GetRelativePath(".uss");
        private static string UxmlPath => GetRelativePath(".uxml");

        private static string GetRelativePath(string extension)
        {
            var guids = AssetDatabase.FindAssets("BuildProfileWindowExtender t:Script");

            if (guids.Length > 0)
            {
                string scriptPath = AssetDatabase.GUIDToAssetPath(guids[0]);
                return scriptPath.Replace(".cs", extension);
            }

            return string.Empty;
        }

        static BuildProfileWindowExtender()
        {
            EditorApplication.update += OnUpdate;
        }

        private static void OnUpdate()
        {
            bool IsProfileWindow(EditorWindow window)
            {
                return window.GetType().FullName == WINDOW_TYPE_NAME;
            }

            var windows = Resources.FindObjectsOfTypeAll<EditorWindow>();
            var profileWindow = windows.FirstOrDefault(IsProfileWindow);

            if (profileWindow != null)
            {
                InjectToSceneList(profileWindow);
            }
        }

        private static void InjectToSceneList(EditorWindow window)
        {
            var root = window.rootVisualElement;
            
            string ussPath = UssPath;
            if (!string.IsNullOrEmpty(ussPath))
            {
                var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(ussPath);
                if (styleSheet != null && !root.styleSheets.Contains(styleSheet))
                {
                    root.styleSheets.Add(styleSheet);
                }
            }

            var container = root.Q("scene-list-foldout-root") ?? root.Q("scene-list-foldout")?.Q("unity-content");
            if (container == null) return;

            var existing = container.Q("bootstrapper-header-ext");
            if (existing != null)
            {
                UpdateStatus(existing.Q<Label>(className: "bootstrapper-status"));
                return;
            }

            // -------------------------------------------------------------
            /// UXML 로드 및 클론
            // -------------------------------------------------------------
            string uxmlPath = UxmlPath;
            if (string.IsNullOrEmpty(uxmlPath)) return;

            var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(uxmlPath);
            if (visualTree == null) return;

            var headerExt = visualTree.CloneTree().Q("bootstrapper-header-ext");
            if (headerExt == null) return;

            // UI 요소 쿼리 및 이벤트 바인딩
            var statusLabel = headerExt.Q<Label>("status");
            
            headerExt.Q<Button>("btn-create").clicked += () => CreateAndRegisterBootstrapper(window);
            headerExt.Q<Button>("btn-select-boot").clicked += () => ShowSceneSelectionMenu(statusLabel, true);
            headerExt.Q<Button>("btn-select-init").clicked += () => ShowSceneSelectionMenu(statusLabel, false);
            headerExt.Q<Button>("btn-config").clicked += () => SettingsService.OpenProjectSettings("Project/Bootstrapper");

            container.Insert(0, headerExt);

            UpdateStatus(statusLabel);
        }

        private static void CreateAndRegisterBootstrapper(EditorWindow window)
        {
            string scenePath = "Assets/Bootstrapper.unity";

            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset == null)
            {
                var newScene = EditorSceneManager.NewScene(NewSceneSetup.DefaultGameObjects, NewSceneMode.Additive);

                EditorSceneManager.SaveScene(newScene, scenePath);
                EditorSceneManager.CloseScene(newScene, true);

                AssetDatabase.ImportAsset(scenePath);
            }

            var list = new List<EditorBuildSettingsScene>();

            var bootstrapperScene = new EditorBuildSettingsScene(scenePath, true);
            list.Add(bootstrapperScene);

            foreach (var scene in EditorBuildSettings.scenes)
            {
                if (scene.path == scenePath) continue;
                list.Add(scene);
            }

            EditorBuildSettings.scenes = list.ToArray();
            
            SaveBootstrapper(0);

            Debug.Log("[Bootstrapper] Registered index 0.");

            window.Repaint();
        }

        private static void ShowSceneSelectionMenu(Label statusLabel, bool isBootstrapper)
        {
            var scenes = EditorBuildSettings.scenes;
            if (scenes == null || scenes.Length == 0) return;

            var menu = new GenericMenu();
            for (int i = 0; i < scenes.Length; i++)
            {
                int index = i;
                string name = Path.GetFileNameWithoutExtension(scenes[index].path);

                void OnClick()
                {
                    SaveSceneIndex(index, isBootstrapper);
                    UpdateStatus(statusLabel);
                }

                menu.AddItem(new GUIContent($"[{index}] {name}"), false, OnClick);
            }

            menu.ShowAsContext();
        }

        private static void SaveSceneIndex(int sceneIndex, bool isBootstrapper)
        {
            var settings = BootstrapperSettings.Instance;

            if (settings != null)
            {
                if (isBootstrapper)
                {
                    settings.BootstrapperSceneIndex = new XNullable<int>(sceneIndex);
                }
                else
                {
                    settings.SceneIndexToLoad = new XNullable<int>(sceneIndex);
                }

                EditorUtility.SetDirty(settings);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private static void SaveBootstrapper(int sceneIndex)
        {
            SaveSceneIndex(sceneIndex, true);
        }

        private static void UpdateStatus(Label label)
        {
            var settings = BootstrapperSettings.Instance;

            if (settings == null)
            {
                label.text = "N/A";
                return;
            }

            var scenes = EditorBuildSettings.scenes;

            var bootIndex = settings.BootstrapperSceneIndex;
            var initIndex = settings.SceneIndexToLoad;
            
            string bootText = GetSceneNameWithIndex(scenes, bootIndex);
            string initText = GetSceneNameWithIndex(scenes, initIndex);

            if (!bootIndex.HasValue && !initIndex.HasValue)
            {
                label.text = "🚀 No Scene Selected";
            }
            else
            {
                if (bootIndex == initIndex)
                {
                    label.text = $"🚀 Error: Same Index ( {bootText} / {initText} )";
                }
                else
                {
                    label.text = $"🚀 {bootText} → {initText}";
                }
            }
        }

        private static string GetSceneNameWithIndex(EditorBuildSettingsScene[] scenes, XNullable<int> nullableIndex)
        {
            if (!nullableIndex.HasValue) return "N/A";
            
            int index = nullableIndex.Value;
            if (scenes.CheckInRange(index))
            {
                return $"[{index}] {Path.GetFileNameWithoutExtension(scenes[index].path)}";
            }
            
            return "Invalid Index";
        }
    }
}
#endif
