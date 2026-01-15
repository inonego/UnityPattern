#if UNITY_EDITOR

using System;
using System.IO;

using UnityEngine;
using UnityEditor;

namespace inonego
{

    public class AssetUtility : MonoBehaviour
    {

        public static void MakeDirectory(string path)
        {
            if (AssetDatabase.IsValidFolder(path)) return;

            var (parent, current) = (Path.GetDirectoryName(path), Path.GetFileName(path));

            if (!AssetDatabase.IsValidFolder(parent))
            { 
                // 재귀적으로 부모 폴더 생성
                MakeDirectory(parent);
            }

            AssetDatabase.CreateFolder(parent, current);
        }
        
        public static T Load<T>(string directory, string file) where T : ScriptableObject
        {
            var path = directory + file;

            var asset = AssetDatabase.LoadAssetAtPath<T>(path);

            // 파일이 없으면 생성
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<T>();

                MakeDirectory(directory);

                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }

            return asset;
        }

    }
}

#endif