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

            #if UNITY_EDITOR

                if (AssetDatabase.IsValidFolder(path)) return;

                var (parent, current) = (Path.GetDirectoryName(path), Path.GetFileName(path));

                if (!AssetDatabase.IsValidFolder(parent))
                { 
                    // 재귀적으로 부모 폴더 생성
                    MakeDirectory(parent);
                }

                AssetDatabase.CreateFolder(parent, current);

            #endif
        }
    }
}
