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
        private GUIStyle lTextStyle = null;

        private float iconSize = EditorGUIUtility.singleLineHeight;

        private float iconPadding = 4f;
        private float lTextPadding = 4f;
        private float defaultFieldSpacing = 2f;
        private float fieldSpacing = 2f;

        private float iconAndTextPadding => iconPadding + iconSize + lTextPadding;

        private void InitializeStyles()
        {
            if (lTextStyle == null)
            {
                lTextStyle = new GUIStyle(EditorStyles.label)
                {
                    fontSize = 11, wordWrap = true
                };
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;

            InitializeStyles();

            // HelpBox 높이 계산
            var helpBoxHeight = GetHelpBoxHeight(helpBoxAttribute);
            
            var helpBoxRect = new Rect(position.x, position.y, position.width, helpBoxHeight);
            var fieldRect = new Rect(position.x, position.y + helpBoxHeight + fieldSpacing + defaultFieldSpacing, position.width, EditorGUIUtility.singleLineHeight);

            // 커스텀 HelpBox 그리기
            DrawCustomHelpBox(helpBoxRect, helpBoxAttribute.Message, helpBoxAttribute.MessageType, helpBoxAttribute.CustomIconName);
            
            EditorGUI.BeginProperty(fieldRect, label, property);
            EditorGUI.PropertyField(fieldRect, property, label, true);
            EditorGUI.EndProperty();
        }

        private void DrawCustomHelpBox(Rect position, string message, MessageType messageType, string customIconName)
        {
            // -------------------------------------------------------------
            // 배경 그리기
            // - MessageType에 따른 색상
            // -------------------------------------------------------------
            var backgroundColor = GetBackgroundColor(messageType);

            EditorGUI.DrawRect(position, backgroundColor);

            // 아이콘이 있는지 확인
            Texture2D icon = GetIcon(messageType, customIconName);
            
            bool hasIcon = icon != null;

            if (hasIcon)
            {
                // -------------------------------------------------------------
                // 아이콘 위치 계산
                // - 세로 중앙 정렬
                // -------------------------------------------------------------
                var iconX = position.x + iconPadding;
                var iconY = position.y + (position.height - iconSize) * 0.5f;

                var iconRect = new Rect(iconX, iconY, iconSize, iconSize);
                
                // -------------------------------------------------------------
                // 아이콘 그리기
                // -------------------------------------------------------------
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            }

            // -------------------------------------------------------------
            // 텍스트 그리기
            // -------------------------------------------------------------
            float lTextX, lTextWidth;

            if (hasIcon)
            {
                // 아이콘이 있는 경우: 아이콘 오른쪽부터 시작
                lTextX = position.x + iconAndTextPadding;
                lTextWidth = position.width - (iconAndTextPadding + lTextPadding);
            }
            else
            {
                // 아이콘이 없는 경우: 전체 너비 사용
                lTextX = position.x + lTextPadding;
                lTextWidth = position.width - lTextPadding * 2;
            }

            var lTextY = position.y + lTextPadding;
            var lTextHeight = position.height - lTextPadding * 2;

            var lTextRect = new Rect(lTextX, lTextY, lTextWidth, lTextHeight);
            
            GUI.Label(lTextRect, message, lTextStyle);
        }

        private Texture2D GetIcon(MessageType messageType, string customIconName)
        {
            if (!string.IsNullOrEmpty(customIconName))
            {
                return EditorGUIUtility.IconContent(customIconName).image as Texture2D;
            }
            
            var iconName = messageType switch
            {
                MessageType.Info => "console.infoicon",
                MessageType.Warning => "console.warnicon",
                MessageType.Error => "console.erroricon",
                _ => null
            };

            if (iconName != null)
            {
                return EditorGUIUtility.IconContent(iconName).image as Texture2D;
            }

            return null;
        }

        private Color GetBackgroundColor(MessageType messageType)
        {
            return messageType switch
            {
                MessageType.None    => new Color(0.7f, 0.7f, 0.7f, 0.1f),
                MessageType.Info    => new Color(0.5f, 0.7f, 1.0f, 0.1f),
                MessageType.Warning => new Color(1.0f, 0.8f, 0.3f, 0.1f),
                MessageType.Error   => new Color(1.0f, 0.3f, 0.3f, 0.1f),
                _ => Color.white
            };
        }

        private float GetHelpBoxHeight(HelpBoxAttribute helpBoxAttribute)
        {
            InitializeStyles();

            var content = new GUIContent(helpBoxAttribute.Message);
            
            // 아이콘이 있는지 확인
            Texture2D icon = GetIcon(helpBoxAttribute.MessageType, helpBoxAttribute.CustomIconName);

            bool hasIcon = icon != null;
            
            float width;

            if (hasIcon)
            {
                width = EditorGUIUtility.currentViewWidth - (iconAndTextPadding + lTextPadding);
            }
            else
            {
                width = EditorGUIUtility.currentViewWidth - lTextPadding * 2;
            }
            
            var lTextSize = lTextStyle.CalcHeight(content, width);
            
            var textHeight = lTextSize + lTextPadding * 2;
            var iconHeight = hasIcon ? iconSize + iconPadding * 2 : 0;
            
            return Mathf.Max(textHeight, iconHeight);
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var helpBoxAttribute = (HelpBoxAttribute)attribute;
            
            // HelpBox 높이 + 간격 + 원래 필드 높이
            var helpBoxHeight = GetHelpBoxHeight(helpBoxAttribute);
            var fieldHeight = EditorGUI.GetPropertyHeight(property, label, true);
            
            return helpBoxHeight + fieldSpacing + defaultFieldSpacing + fieldHeight + fieldSpacing;
        }
    }

#endif

}
