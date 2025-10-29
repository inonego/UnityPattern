using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace inonego.Editor
{

#if UNITY_EDITOR

    using Serializable;

    // ==============================================================
    /// <summary>
    /// XNullable의 PropertyDrawer입니다.
    /// </summary>
    // ==============================================================
    [CustomPropertyDrawer(typeof(XNullable<>))]
    public class XNullableDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty hasValueProp = property.FindPropertyRelative("hasValue");
            SerializedProperty valueProp = property.FindPropertyRelative("value");

            // 전체 영역을 3등분 (라벨, 값, 체크박스)
            float labelWidth = EditorGUIUtility.labelWidth - 2;
            float checkboxWidth = 16f;
            float spacing = 4f;

            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            Rect valueRect = new Rect(position.x + labelWidth + spacing, position.y, position.width - (labelWidth + checkboxWidth + spacing * 2), position.height);
            Rect checkboxRect = new Rect(position.x + position.width - checkboxWidth, position.y, checkboxWidth, position.height);

            // 라벨 표시
            EditorGUI.LabelField(labelRect, label);

            EditorGUI.indentLevel = 0;

            bool enabled = GUI.enabled;

            GUI.enabled = hasValueProp.boolValue;
            EditorGUI.PropertyField(valueRect, valueProp, GUIContent.none, true);
            GUI.enabled = enabled;

            // HasValue 토글 표시
            EditorGUI.BeginChangeCheck();
            bool hasValue = EditorGUI.Toggle(checkboxRect, hasValueProp.boolValue);
            if (EditorGUI.EndChangeCheck())
            {
                hasValueProp.boolValue = hasValue;
            }

            EditorGUI.EndProperty();
        }
    }

#endif

}