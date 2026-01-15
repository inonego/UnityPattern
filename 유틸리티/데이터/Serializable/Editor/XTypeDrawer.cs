using System;
using System.Reflection;

using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.IMGUI;
using UnityEditor.IMGUI.Controls;

namespace inonego.Editor
{
    using inonego.Serializable;

    // ==============================================================
    /// <summary>
    /// XType의 PropertyDrawer입니다.
    /// </summary>
    // ==============================================================
    [CustomPropertyDrawer(typeof(XType))]
    public class XTypeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty nameProp = property.FindPropertyRelative("name");

            string fullName = nameProp?.stringValue ?? string.Empty;

            // 라벨 그리기
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            string displayText = "None";
            if (!string.IsNullOrEmpty(fullName))
            {
                Type type = Type.GetType(fullName);

                if (type != null)
                {
                    if (string.IsNullOrEmpty(type.Namespace))
                    {
                        displayText = type.Name;
                    }
                    else
                    {
                        displayText = $"{type.Name} ({type.Namespace})";
                    }
                }
                else
                {
                    displayText = $"Invalid: {fullName}";
                }
            }

            if (GUI.Button(position, new GUIContent(displayText, fullName), EditorStyles.popup))
            {
                var filterAttribute = fieldInfo.GetCustomAttribute<XTypeFilterAttribute>();
                
                TypeFilter filter = null;
                
                if (filterAttribute != null)
                {
                    filter = new TypeFilter
                    {
                        BaseType = filterAttribute.BaseType,
                        Assembly = filterAttribute.Assembly,
                        Namespace = filterAttribute.Namespace,
                        
                        Group = filterAttribute.Group
                    };
                }

                void OnSelected(Type selectedType)
                {
                    if (nameProp != null)
                    {
                        nameProp.stringValue = selectedType.AssemblyQualifiedName;
                        property.serializedObject.ApplyModifiedProperties();
                    }
                }

                var dropdown = new TypeAdvancedDropdown(new AdvancedDropdownState(), filter, OnSelected);

                dropdown.Show(position);
            }

            EditorGUI.EndProperty();
        }
    }
}

#endif