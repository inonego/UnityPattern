using System;

using UnityEditor;

#if UNITY_EDITOR
using UnityEngine;
#endif

namespace inonego.Editor
{

#if UNITY_EDITOR

   // ============================================================
   /// <summary>
   /// IGameObjectProvider 인터페이스에 대한 프로퍼티 드로어입니다.
   /// </summary>
   // ============================================================
   [CustomPropertyDrawer(typeof(IGameObjectProvider), false)]
   public class GameObjectProviderDrawer : PropertyDrawer
   {

    #region 상수

        private const float BUTTON_HEIGHT = 18f;
        private const float SPACING = 2f;
        private const float BUTTON_SPACING = 4f;

    #endregion

    #region Unity 오버라이드

        // ------------------------------------------------------------
        /// <summary>
        /// 프로퍼티의 GUI를 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // 필드가 IGameObjectProvider로 선언되지 않은 경우 - 기본 드로어만 사용
            if (!IsInterfaceField())
            {
                EditorGUI.PropertyField(position, property, label, true);
                
                return;
            }

            // SerializeReference가 아니면 - 기본 드로어만 사용
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                EditorGUI.PropertyField(position, property, label, true);
                
                return;
            }

            EditorGUI.BeginProperty(position, label, property);

            // 현재 값 가져오기
            var currentValue = property.managedReferenceValue;
            
            // 라벨 영역
            Rect labelRect = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);

            property.isExpanded = EditorGUI.Foldout(labelRect, property.isExpanded, label, true);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel++;
                
                float yOffset = position.y + EditorGUIUtility.singleLineHeight + SPACING;

                var isPrefab = currentValue is PrefabGameObjectProvider;
                var isAddressable = currentValue is AddressableGameObjectProvider;

                // null이거나 두 타입이 아닌 경우: 두 버튼 나란히 배치
                if (currentValue == null || !(isPrefab || isAddressable))
                {
                    Rect buttonAreaRect = new Rect(position.x, yOffset, position.width, BUTTON_HEIGHT);
                    DrawTypeSelectionButtons(buttonAreaRect, property);
                }
                else
                {
                    // 타입 전환 버튼 그리기
                    Rect switchButtonRect = new Rect(position.x, yOffset, position.width, BUTTON_HEIGHT);
                    DrawTypeSwitchButton(switchButtonRect, property, currentValue);
                    
                    yOffset += BUTTON_HEIGHT + SPACING;

                    // 필드들 그리기
                    DrawProviderFields(position, property, ref yOffset);
                }

                EditorGUI.indentLevel--;
            }

            EditorGUI.EndProperty();
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 프로퍼티의 높이를 계산합니다.
        /// </summary>
        // ------------------------------------------------------------
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            // 필드가 IGameObjectProvider로 선언되지 않은 경우 - 기본 높이만 반환
            if (!IsInterfaceField())
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            // SerializeReference가 아니면 - 기본 높이만 반환
            if (property.propertyType != SerializedPropertyType.ManagedReference)
            {
                return EditorGUI.GetPropertyHeight(property, label, true);
            }

            float height = EditorGUIUtility.singleLineHeight;

            if (!property.isExpanded)
            {
                return height;
            }

            // 버튼 높이 추가
            height += BUTTON_HEIGHT + SPACING;

            var currentValue = property.managedReferenceValue;

            var isPrefab = currentValue is PrefabGameObjectProvider;
            var isAddressable = currentValue is AddressableGameObjectProvider;
            
            // null이거나 두 타입이 아닌 경우는 버튼만 표시
            if (currentValue == null || !(isPrefab || isAddressable))
            {
                return height;
            }

            // 필드들의 높이 계산
            height += GetProviderFieldsHeight(property);

            return height;
        }

    #endregion

    #region 버튼 그리기

        // ------------------------------------------------------------
        /// <summary>
        /// 타입 선택 버튼 두 개를 나란히 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        private void DrawTypeSelectionButtons(Rect position, SerializedProperty property)
        {
            Rect indented = GetChildContentRect(position);
            float buttonWidth = (indented.width - BUTTON_SPACING) / 2f;

            Rect prefabButtonRect = new Rect(indented.x, indented.y, buttonWidth, BUTTON_HEIGHT);
            Rect addressableButtonRect = new Rect(indented.x + buttonWidth + BUTTON_SPACING, indented.y, buttonWidth, BUTTON_HEIGHT);

            if (GUI.Button(prefabButtonRect, "Prefab"))
            {
                property.managedReferenceValue = new PrefabGameObjectProvider();
                property.serializedObject.ApplyModifiedProperties();
            }

            if (GUI.Button(addressableButtonRect, "Addressable"))
            {
                property.managedReferenceValue = new AddressableGameObjectProvider();
                property.serializedObject.ApplyModifiedProperties();
            }
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 타입 전환 버튼을 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        private void DrawTypeSwitchButton(Rect position, SerializedProperty property, object currentValue)
        {
            string buttonText = "";
            Type targetType = null;
            Rect indented = GetChildContentRect(position);

            if (currentValue is PrefabGameObjectProvider)
            {
                buttonText = "→ Addressable";
                targetType = typeof(AddressableGameObjectProvider);
            }
            else if (currentValue is AddressableGameObjectProvider)
            {
                buttonText = "→ Prefab";
                targetType = typeof(PrefabGameObjectProvider);
            }

            if (!string.IsNullOrEmpty(buttonText) && GUI.Button(indented, buttonText))
            {
                property.managedReferenceValue = Activator.CreateInstance(targetType);
                property.serializedObject.ApplyModifiedProperties();
            }
        }

    #endregion

    #region 필드 그리기

        // ------------------------------------------------------------
        /// <summary>
        /// Provider의 필드들의 높이를 계산합니다.
        /// </summary>
        // ------------------------------------------------------------
        private float GetProviderFieldsHeight(SerializedProperty property)
        {
            float height = 0f;

            int depth = property.depth;

            SerializedProperty iterator = property.Copy();
            
            if (iterator.NextVisible(true))
            {
                do
                {
                    if (iterator.depth <= depth) break;

                    height += EditorGUI.GetPropertyHeight(iterator, true) + SPACING;
                }
                while (iterator.NextVisible(false));
            }

            return height;
        }

        // ------------------------------------------------------------
        /// <summary>
        /// Provider의 필드들을 그립니다.
        /// </summary>
        // ------------------------------------------------------------
        private void DrawProviderFields(Rect position, SerializedProperty property, ref float yOffset)
        {
            int depth = property.depth;
            
            SerializedProperty iterator = property.Copy();
            
            if (!iterator.NextVisible(true)) return;

            do
            {
                if (iterator.depth <= depth) break;

                float fieldHeight = EditorGUI.GetPropertyHeight(iterator, true);
                Rect fieldRect = new Rect(position.x, yOffset, position.width, fieldHeight);
                EditorGUI.PropertyField(fieldRect, iterator, true);
                yOffset += fieldHeight + SPACING;
            }
            while (iterator.NextVisible(false));
        }

    #endregion

    #region 유틸리티

        // ------------------------------------------------------------
        /// <summary>
        /// 자식 콘텐츠(필드 수준)와 같은 들여쓰기를 적용한 Rect를 반환합니다.
        /// </summary>
        // ------------------------------------------------------------
        private static Rect GetChildContentRect(Rect position)
        {
            // 현재 indentLevel에 맞춘 기본 들여쓰기만 적용
            return EditorGUI.IndentedRect(position);
        }

        // ------------------------------------------------------------
        /// <summary>
        /// 필드가 IGameObjectProvider 인터페이스 타입으로 선언되었는지 확인합니다.
        /// </summary>
        // ------------------------------------------------------------
        private bool IsInterfaceField()
        {
            if (fieldInfo == null) return false;

            return fieldInfo.FieldType.IsInterface && fieldInfo.FieldType == typeof(IGameObjectProvider);
        }

    #endregion

    }

#endif

}