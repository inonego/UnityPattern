using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace inonego.Editor
{

#if UNITY_EDITOR

    // ==============================================================
    /// <summary>
    /// SerializedProperty 관련 유틸리티 메서드를 제공합니다.
    /// </summary>
    // ==============================================================
    public static class SerializedPropertyUtility
    {
        // ------------------------------------------------------------
        /// <summary>
        /// SerializedProperty로부터 실제 객체를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public static object GetTargetObjectOfProperty(SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");
            object obj = property.serializedObject.targetObject;
            var elements = path.Split('.');
            
            foreach (var element in elements)
            {
                if (element.Contains("["))
                {
                    var elementName = element.Substring(0, element.IndexOf("["));
                    var index = int.Parse(element.Substring(element.IndexOf("[")).Replace("[", "").Replace("]", ""));
                    obj = GetValue(obj, elementName, index);
                }
                else
                {
                    obj = GetValue(obj, element);
                }
            }
            return obj;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 리플렉션을 통해 객체의 필드나 프로퍼티 값을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        private static object GetValue(object source, string name)
        {
            if (source == null) return null;
            var type = source.GetType();

            while (type != null)
            {
                var f = type.GetField(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
                if (f != null) return f.GetValue(source);

                var p = type.GetProperty(name, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (p != null) return p.GetValue(source, null);

                type = type.BaseType;
            }
            return null;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 리플렉션을 통해 컬렉션의 특정 인덱스 값을 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        private static object GetValue(object source, string name, int index)
        {
            var enumerable = GetValue(source, name) as IEnumerable;
            if (enumerable == null) return null;
            var enm = enumerable.GetEnumerator();
            
            for (int i = 0; i <= index; i++)
            {
                if (!enm.MoveNext()) return null;
            }
            return enm.Current;
        }
    }

#endif

}

