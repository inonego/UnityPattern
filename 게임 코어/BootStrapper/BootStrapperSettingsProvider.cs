#if UNITY_EDITOR

using UnityEditor;

namespace inonego.Core.Bootstrapper
{
    using inonego.Editor;
    
    public class BootstrapperSettingsProvider : SettingsProvider
    {
        private const string SETTINGS_MENU_PATH = "Project/Bootstrapper";
        
        private SerializedObject serializedObject;

        // -------------------------------------------------------------
        /// <summary>
        /// SettingsProvider 생성자
        /// </summary>
        // -------------------------------------------------------------
        public BootstrapperSettingsProvider(string path, SettingsScope scope = SettingsScope.Project) : base(path, scope) {}

        // -------------------------------------------------------------
        /// <summary>
        /// SettingsProvider 생성 및 등록
        /// </summary>
        // -------------------------------------------------------------
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            var provider = new BootstrapperSettingsProvider(SETTINGS_MENU_PATH, SettingsScope.Project)
            {
                keywords = GetSearchKeywordsFromGUIContentProperties<BootstrapperSettings>()
            };

            return provider;
        }

        // -------------------------------------------------------------
        /// <summary>
        /// SettingsProvider 활성화 시 호출
        /// </summary>
        // -------------------------------------------------------------
        public override void OnActivate(string searchContext, UnityEngine.UIElements.VisualElement rootElement)
        {
            base.OnActivate(searchContext, rootElement);

            var instance = BootstrapperSettings.Instance;

            serializedObject = new SerializedObject(instance);
        }

        public override void OnGUI(string searchContext) 
        {
            serializedObject.Update();
            SerializedObjectUtility.DrawAll(serializedObject);
            serializedObject.ApplyModifiedProperties();
        }
    }
}

#endif
