using UnityEngine;
using UnityEditor;

namespace inonego
{
    //===============================================================
    /// <summary>
    /// HelpBox를 그리는 유틸리티 클래스
    /// </summary>
    //===============================================================
    public static class HelpBoxUtility
    {
        
    #region 상수

        private const float ICON_SIZE = 16f;
        private const float ICON_PADDING = 4f;
        private const float TEXT_PADDING = 4f;

    #endregion

    #region 스타일

        private static GUIStyle lTextStyle;
        private static GUIStyle TextStyle
        {
            get
            {
                if (lTextStyle == null)
                {
                    lTextStyle = new GUIStyle(EditorStyles.label)
                    {
                        fontSize = 10,
                        wordWrap = true
                    };
                }

                return lTextStyle;
            }
        }

    #endregion

    #region 프로퍼티

        private static float IconAndTextPadding => ICON_PADDING + ICON_SIZE + TEXT_PADDING;

    #endregion

    #region 공개 메서드

        //===============================================================
        /// <summary>
        /// HelpBox를 그립니다 (MessageType 사용)
        /// </summary>
        /// <param name="position">그릴 위치</param>
        /// <param name="message">표시할 메시지</param>
        /// <param name="messageType">메시지 타입</param>
        //===============================================================
        public static void DrawHelpBox(Rect position, string message, MessageType messageType)
        {
            DrawHelpBoxInternal(position, message, messageType, null);
        }

        //===============================================================
        /// <summary>
        /// HelpBox를 그립니다 (커스텀 아이콘 사용)
        /// </summary>
        /// <param name="position">그릴 위치</param>
        /// <param name="message">표시할 메시지</param>
        /// <param name="customIconName">커스텀 아이콘 이름</param>
        //===============================================================
        public static void DrawHelpBox(Rect position, string message, string customIconName)
        {
            DrawHelpBoxInternal(position, message, MessageType.None, customIconName);
        }
        
        //===============================================================
        /// <summary>
        /// HelpBox의 높이를 계산합니다 (MessageType 사용)
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="messageType">메시지 타입</param>
        /// <param name="width">사용 가능한 너비</param>
        /// <returns>계산된 높이</returns>
        //===============================================================
        public static float GetHelpBoxHeight(string message, MessageType messageType, float width = -1)
        {
            return GetHelpBoxHeightInternal(message, messageType, null, width);
        }

        //===============================================================
        /// <summary>
        /// HelpBox의 높이를 계산합니다 (커스텀 아이콘 사용)
        /// </summary>
        /// <param name="message">표시할 메시지</param>
        /// <param name="customIconName">커스텀 아이콘 이름</param>
        /// <param name="width">사용 가능한 너비</param>
        /// <returns>계산된 높이</returns>
        //===============================================================
        public static float GetHelpBoxHeight(string message, string customIconName, float width = -1)
        {
            return GetHelpBoxHeightInternal(message, MessageType.None, customIconName, width);
        }

    #endregion

    #region 메서드

        internal static void DrawHelpBoxInternal(Rect position, string message, MessageType messageType, string customIconName)
        {
            // ------------------------------------------------------------
            // 배경 그리기
            // ------------------------------------------------------------
            var backgroundColor = GetBackgroundColor(messageType);
            EditorGUI.DrawRect(position, backgroundColor);

            // ------------------------------------------------------------
            // 아이콘 그리기
            // ------------------------------------------------------------
            Texture2D icon = GetIcon(messageType, customIconName);

            bool hasIcon = icon != null;

            if (hasIcon)
            {
                var iconX = position.x + ICON_PADDING;
                var iconY = position.y + (position.height - ICON_SIZE) * 0.5f;
                var iconRect = new Rect(iconX, iconY, ICON_SIZE, ICON_SIZE);
                
                GUI.DrawTexture(iconRect, icon, ScaleMode.ScaleToFit);
            }

            // ------------------------------------------------------------
            // 텍스트 그리기
            // ------------------------------------------------------------
            float textX, textWidth;

            if (hasIcon)
            {
                textX = position.x + IconAndTextPadding;
                textWidth = position.width - (IconAndTextPadding + TEXT_PADDING);
            }
            else
            {
                textX = position.x + TEXT_PADDING;
                textWidth = position.width - TEXT_PADDING * 2;
            }

            var textY = position.y + TEXT_PADDING;
            var textHeight = position.height - TEXT_PADDING * 2 - 1;
            var textRect = new Rect(textX, textY, textWidth, textHeight);
            
            GUI.Label(textRect, message, TextStyle);
        }

        internal static float GetHelpBoxHeightInternal(string message, MessageType messageType, string customIconName, float width = -1)
        {
            if (width < 0)
            {
                width = EditorGUIUtility.currentViewWidth;
            }

            var content = new GUIContent(message);
            
            // ------------------------------------------------------------
            // 아이콘 확인
            // ------------------------------------------------------------
            Texture2D icon = GetIcon(messageType, customIconName);

            bool hasIcon = icon != null;
            
            float availableWidth;

            if (hasIcon)
            {
                availableWidth = width - (IconAndTextPadding + TEXT_PADDING);
            }
            else
            {
                availableWidth = width - TEXT_PADDING * 2;
            }
            
            var textSize = TextStyle.CalcHeight(content, availableWidth);
            
            var textHeight = textSize + TEXT_PADDING * 2 - 1;
            var iconHeight = hasIcon ? ICON_SIZE + ICON_PADDING * 2 : 0;
            
            return Mathf.Max(textHeight, iconHeight) + 1;
        }

        internal static Texture2D GetIcon(MessageType messageType, string customIconName)
        {
            // 커스텀 아이콘이 있으면 우선 사용
            if (!string.IsNullOrEmpty(customIconName))
            {
                return EditorGUIUtility.IconContent(customIconName).image as Texture2D;
            }
            
            // MessageType에 따른 기본 아이콘 사용
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

        internal static Color GetBackgroundColor(MessageType messageType)
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

    #endregion

    }
}
