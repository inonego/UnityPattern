using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace inonego
{
    public class HelpBoxAttribute : PropertyAttribute
    {
        public string Message { get; }
        public string CustomIconName { get; }
        public MessageType MessageType { get; }

        public HelpBoxAttribute(string message, MessageType messageType = MessageType.Info)
        {
            Message = message;
            MessageType = messageType;
            CustomIconName = null;
        }

        public HelpBoxAttribute(string message, string iconName)
        {
            Message = message;
            MessageType = MessageType.None;
            CustomIconName = iconName;
        }
    }

#if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(HelpBoxAttribute))]
    public class HelpBoxDrawer : PropertyDrawer
    {
        private float defaultFieldSpacing = 2f;
        private float fieldSpacing = 2f;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;

            // HelpBox 높이 계산
            var helpBoxHeight = HelpBoxUtility.GetHelpBoxHeightInternal(helpBoxAttribute.Message, helpBoxAttribute.MessageType, helpBoxAttribute.CustomIconName, position.width);
            
            var helpBoxRect = new Rect(position.x, position.y, position.width, helpBoxHeight);
            var fieldRect = new Rect(position.x, position.y + helpBoxHeight + fieldSpacing + defaultFieldSpacing, position.width, EditorGUIUtility.singleLineHeight);

            // HelpBox 그리기
            HelpBoxUtility.DrawHelpBoxInternal(helpBoxRect, helpBoxAttribute.Message, helpBoxAttribute.MessageType, helpBoxAttribute.CustomIconName);
            
            EditorGUI.BeginProperty(fieldRect, label, property);
            EditorGUI.PropertyField(fieldRect, property, label, true);
            EditorGUI.EndProperty();
        }


        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            
            // HelpBox 높이 + 간격 + 원래 필드 높이
            var helpBoxHeight = HelpBoxUtility.GetHelpBoxHeightInternal(helpBoxAttribute.Message, helpBoxAttribute.MessageType, helpBoxAttribute.CustomIconName);
            var fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);
            
            return helpBoxHeight + fieldSpacing + defaultFieldSpacing + fieldHeight + fieldSpacing;
        }
    }

#endif

}
