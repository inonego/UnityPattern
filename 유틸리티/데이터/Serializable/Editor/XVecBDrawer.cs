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
    /// XVecB 타입의 공통 그리기 헬퍼 클래스입니다.
    /// </summary>
    // ==============================================================
    internal static class XVecBDrawerHelper
    {
        private const float CheckboxWidth = 16f;
        private const float AxisLabelWidth = 15f;
        private const float Spacing = 4f;
        private const float AxisSpacing = 10f;

        // ------------------------------------------------------------
        /// <summary>
        /// 축 라벨과 체크박스를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public static float DrawAxisToggle(Rect position, string axisName, SerializedProperty axisProp, float startX)
        {
            Rect labelRect = new Rect(startX, position.y, AxisLabelWidth, position.height);
            EditorGUI.LabelField(labelRect, axisName);

            Rect toggleRect = new Rect(startX + AxisLabelWidth + 2, position.y, CheckboxWidth, position.height);
            axisProp.boolValue = EditorGUI.Toggle(toggleRect, axisProp.boolValue);

            return startX + AxisLabelWidth + CheckboxWidth + Spacing + AxisSpacing;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 공통 초기 설정을 수행하고 콘텐츠 시작 위치를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        public static float DrawLabelAndGetContentX(Rect position, GUIContent label)
        {
            // 라벨이 비어있으면 (XNullable 등에서 GUIContent.none을 전달한 경우) 라벨을 그리지 않음
            if (label == GUIContent.none || string.IsNullOrEmpty(label.text))
            {
                return position.x;
            }

            float labelWidth = EditorGUIUtility.labelWidth;
            Rect labelRect = new Rect(position.x, position.y, labelWidth, position.height);
            EditorGUI.LabelField(labelRect, label);
            return position.x + labelWidth + Spacing;
        }
    }

    // ==============================================================
    /// <summary>
    /// XVec2B의 PropertyDrawer입니다.
    /// </summary>
    // ==============================================================
    [CustomPropertyDrawer(typeof(XVec2B))]
    public class XVec2BDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty xProp = property.FindPropertyRelative("X");
            SerializedProperty yProp = property.FindPropertyRelative("Y");

            float content = XVecBDrawerHelper.DrawLabelAndGetContentX(position, label);

            content = XVecBDrawerHelper.DrawAxisToggle(position, "X", xProp, content);
            content = XVecBDrawerHelper.DrawAxisToggle(position, "Y", yProp, content);

            EditorGUI.EndProperty();
        }
    }

    // ==============================================================
    /// <summary>
    /// XVec3B의 PropertyDrawer입니다.
    /// </summary>
    // ==============================================================
    [CustomPropertyDrawer(typeof(XVec3B))]
    public class XVec3BDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty xProp = property.FindPropertyRelative("X");
            SerializedProperty yProp = property.FindPropertyRelative("Y");
            SerializedProperty zProp = property.FindPropertyRelative("Z");

            float content = XVecBDrawerHelper.DrawLabelAndGetContentX(position, label);

            content = XVecBDrawerHelper.DrawAxisToggle(position, "X", xProp, content);
            content = XVecBDrawerHelper.DrawAxisToggle(position, "Y", yProp, content);
            content = XVecBDrawerHelper.DrawAxisToggle(position, "Z", zProp, content);

            EditorGUI.EndProperty();
        }
    }

#endif

}

