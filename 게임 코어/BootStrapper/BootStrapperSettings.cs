using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace inonego.Core.Bootstrapper 
{
    using Serializable;

    public class BootstrapperSettings : ScriptableObject
    {        
        private const string RESOURCES_DIRECTORY = "Assets/Resources/";
        private const string RESOURCES_FILE = "BootstrapperSettings";

        private static BootstrapperSettings instance;
        public static BootstrapperSettings Instance
        {
            get
            {
                if (instance == null)
                {
                    
                #if UNITY_EDITOR
                    instance = AssetUtility.Load<BootstrapperSettings>(RESOURCES_DIRECTORY, $"{RESOURCES_FILE}.asset");
                #else
                    instance = Resources.Load<BootstrapperSettings>(RESOURCES_FILE);
                #endif

                }

                return instance;
            }
        }

        [HelpBox("부트스트래퍼의 씬 인덱스입니다.")]
        public XNullable<int> BootstrapperSceneIndex = null;

        [HelpBox("부트스트래퍼 실행이 완료된 후 로드할 실제 게임 시작 씬 인덱스입니다.")]
        public XNullable<int> SceneIndexToLoad = null;

        [Serializable]
        public class ModuleScript
        {
            [XTypeFilter(BaseType = typeof(IBootstrapperModule), Group = false)]
            public XType Type;
        }

        [Header("Modules")]
        public List<ModuleScript> ModuleScripts = new();
    }
}
