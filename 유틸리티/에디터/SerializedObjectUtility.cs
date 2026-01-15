using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text.RegularExpressions;

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
    public static class SerializedObjectUtility
    {
        // ------------------------------------------------------------
        /// <summary>
        /// SerializedProperty로부터 실제 객체를 가져옵니다.
        /// </summary>
        // ------------------------------------------------------------
        public static object GetTargetObject(this SerializedProperty property)
        {
            var path = property.propertyPath.Replace(".Array.data[", "[");
            var elements = path.Split('.');

            // 경로를 '.'으로 분리하고 각각의 요소를 처리하면서 실제 객체를 찾습니다.
            
            object @object = property.serializedObject.targetObject;

            foreach (var element in elements)
            {
                var match = Regex.Match(element, @"^(\w+)\[(\d+)\]$");

                if (match.Success)
                {
                    var groups = match.Groups;
                    var (name, rawIndex) = (groups[1].Value, groups[2].Value);
                    var index = int.Parse(rawIndex);

                    @object = GetValue(@object, name, index);
                }
                else
                {
                    @object = GetValue(@object, element);
                }
            }

            return @object;
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
                var flag = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance;
                
                var field = type.GetField(name, flag);
                if (field != null) return field.GetValue(source);

                var property = type.GetProperty(name, flag);
                if (property != null) return property.GetValue(source, null);

                // 찾을 수 없는 경우 상위 타입을 확인합니다.
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
            
            var enumerator = enumerable.GetEnumerator();
            if (enumerator == null) return null;
            
            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext()) return null;
            }

            return enumerator.Current;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// SerializedObject의 모든 프로퍼티를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static void DrawAll(SerializedObject serializedObject)
        {            
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                // m_Script 필드는 건너뛰기
                if (iterator.name == "m_Script")
                {
                    enterChildren = false;
                    continue;
                }

                EditorGUILayout.PropertyField(iterator, true);
                enterChildren = false;
            }
        }
    }

#endif

}

