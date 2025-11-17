using System;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace inonego.Editor
{

#if UNITY_EDITOR

    // ==============================================================
    /// <summary>
    /// RangeValue의 PropertyDrawer 기본 클래스입니다.
    /// </summary>
    // ==============================================================
    public abstract class RangeValuePropertyDrawerBase<T> : PropertyDrawer where T : struct, IComparable<T>
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            // SerializedObject 최신 상태로 업데이트
            property.serializedObject.Update();

            // 리플렉션으로 실제 객체 가져오기
            RangeValue<T> value = SerializedPropertyUtility.GetTargetObjectOfProperty(property) as RangeValue<T>;
            
            if (value == null)
            {
                EditorGUI.EndProperty();
                return;
            }

            // 변수명 표시
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            string niceName = ObjectNames.NicifyVariableName(property.name);
            EditorGUI.LabelField(labelRect, niceName);

            float lineHeight = EditorGUIUtility.singleLineHeight;
            float spacing = 2f;
            float yOffset = lineHeight + spacing;
            float indent = 15f; // 들여쓰기

            // Base 값 (슬라이더로 표시)
            Rect currentLabelRect = new Rect(position.x + indent, position.y + yOffset, 60, lineHeight);
            Rect currentSliderRect = new Rect(position.x + indent + 65, position.y + yOffset, position.width - indent - 65, lineHeight);
            
            EditorGUI.LabelField(currentLabelRect, "Base");
            T newBase = DrawSlider(currentSliderRect, value.Base, value.Min, value.Max);
            
            if (!newBase.Equals(value.Base))
            {
                value.Base = newBase;
                EditorUtility.SetDirty(property.serializedObject.targetObject);
                property.serializedObject.ApplyModifiedProperties();
            }

            yOffset += lineHeight + spacing;

            // Min/Max 값
            Rect rangeLabelRect = new Rect(position.x + indent, position.y + yOffset, 60, lineHeight);
            
            float fieldWidth = (position.width - indent - 100) / 2;
            float labelWidth = 30;
            
            Rect minLabelRect = new Rect(position.x + indent + 65, position.y + yOffset, labelWidth, lineHeight);
            Rect minFieldRect = new Rect(position.x + indent + 65 + labelWidth, position.y + yOffset, fieldWidth, lineHeight);
            Rect maxLabelRect = new Rect(position.x + indent + 65 + labelWidth + fieldWidth + 5, position.y + yOffset, labelWidth, lineHeight);
            Rect maxFieldRect = new Rect(position.x + indent + 65 + labelWidth + fieldWidth + 5 + labelWidth, position.y + yOffset, fieldWidth, lineHeight);

            EditorGUI.LabelField(rangeLabelRect, "Range");
            
            EditorGUI.LabelField(minLabelRect, "Min");
            T newMin = DrawField(minFieldRect, value.Min);
            EditorGUI.LabelField(maxLabelRect, "Max");
            T newMax = DrawField(maxFieldRect, value.Max);

            if (!newMin.Equals(value.Min) || !newMax.Equals(value.Max))
            {
                try
                {
                    value.Range.Base = (newMin, newMax);
                    EditorUtility.SetDirty(property.serializedObject.targetObject);
                    property.serializedObject.ApplyModifiedProperties();
                }
                catch (InvalidOperationException)
                {
                    // 잘못된 범위이므로 무시합니다.
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight * 3 + 4; // 라벨 + Base + Min/Max + 여백
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 슬라이더를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T DrawSlider(Rect rect, T value, T min, T max);

        // ------------------------------------------------------------
        /// <summary>
        /// 필드를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        protected abstract T DrawField(Rect rect, T value);
    }

    [CustomPropertyDrawer(typeof(RangeValue<float>))]
    public class RangeValueFloatPropertyDrawer : RangeValuePropertyDrawerBase<float>
    {
        protected override float DrawSlider(Rect rect, float value, float min, float max)
        {
            return EditorGUI.Slider(rect, value, min, max);
        }

        protected override float DrawField(Rect rect, float value)
        {
            return EditorGUI.FloatField(rect, value);
        }
    }

    [CustomPropertyDrawer(typeof(RangeValue<int>))]
    public class RangeValueIntPropertyDrawer : RangeValuePropertyDrawerBase<int>
    {
        protected override int DrawSlider(Rect rect, int value, int min, int max)
        {
            return EditorGUI.IntSlider(rect, value, min, max);
        }

        protected override int DrawField(Rect rect, int value)
        {
            return EditorGUI.IntField(rect, value);
        }
    }

#endif

}
