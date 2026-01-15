using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace inonego.Editor
{

#if UNITY_EDITOR

    // ==============================================================
    /// <summary>
    /// Timer의 PropertyDrawer입니다.
    /// </summary>
    // ==============================================================
    [CustomPropertyDrawer(typeof(Timer))]
    public class TimerPropertyDrawer : PropertyDrawer
    {
        private GUIStyle compactButtonStyle = null;

        // ------------------------------------------------------------
        /// <summary>
        /// Timer 객체의 변경사항을 SerializedProperty에 적용합니다.
        /// </summary>
        // ------------------------------------------------------------
        private void ApplyTimerChanges(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
            GUI.changed = true;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            if (compactButtonStyle == null)
            {
                compactButtonStyle = new(GUI.skin.button)
                {
                    padding = new RectOffset(0, 0, 0, 0),
                    margin = new RectOffset(0, 0, 0, 0)
                };
            }

            // SerializedObject 최신 상태로 업데이트
            property.serializedObject.Update();

            // 리플렉션으로 실제 객체 가져오기
            Timer timer = property.GetTargetObject() as Timer;

            if (timer == null)
            {
                EditorGUI.EndProperty();

                return;
            }

            // 변수명 표시
            Rect nameRect = new Rect(position.x, position.y, position.width - 200, EditorGUIUtility.singleLineHeight);
            string niceName = ObjectNames.NicifyVariableName(property.name);
            EditorGUI.LabelField(nameRect, niceName);

            // 상태 박스와 프로그레스 바
            Rect stateBoxRect = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + 2, 60, 20);
            
            // 컨트롤 영역 크기 계산
            int buttonSize = 20;        // 프로그레스바와 같은 높이
            int buttonSpacing = 2;      // 버튼과 버튼 사이의 여백
            int inputWidth = 45;        // 입력칸 크기 줄임
            int progressSpacing = 8;    // 프로그레스바와의 여백
            int rightMargin = 6;        // 오른쪽 여백
            int totalControlWidth = progressSpacing + inputWidth + buttonSize + buttonSpacing + buttonSize + rightMargin;
            
            // 프로그레스바는 컨트롤 요소들을 제외한 나머지 공간 사용
            Rect progressRect = new Rect(position.x + 65, position.y + EditorGUIUtility.singleLineHeight + 2, position.width - 65 - totalControlWidth, 20);
            
            DrawStateBox(stateBoxRect, timer.Current);
            DrawProgressBar(progressRect, timer);

            // 컨트롤 버튼들
            if (timer.Current == TimerState.Ready)
            {
                // Ready 상태
                
                Rect timeInputRect = new Rect(progressRect.x + progressRect.width + progressSpacing, progressRect.y, inputWidth, progressRect.height);
                Rect startButtonRect = new Rect(progressRect.x + progressRect.width + progressSpacing + inputWidth + buttonSpacing, progressRect.y, buttonSize, buttonSize);
                Rect resetButtonRect = new Rect(progressRect.x + progressRect.width + progressSpacing + inputWidth + buttonSpacing + buttonSize + buttonSpacing, progressRect.y, buttonSize, buttonSize);

                float newDuration = EditorGUI.FloatField(timeInputRect, timer.cachedStartDuration);
                if (newDuration != timer.cachedStartDuration)
                {
                    timer.cachedStartDuration = newDuration;
                    ApplyTimerChanges(property);
                }

                GUI.enabled = newDuration >= 0;
                if (GUI.Button(startButtonRect, EditorGUIUtility.IconContent("d_PlayButton"), compactButtonStyle))
                {
                    timer.Start(newDuration);
                    ApplyTimerChanges(property);
                }
                GUI.enabled = true;

                // Reset 버튼은 타이머가 실행된 적이 있을 때만 활성화
                GUI.enabled = timer.Duration > 0 || timer.ElapsedTime > 0;
                if (GUI.Button(resetButtonRect, EditorGUIUtility.IconContent("d_Refresh"), compactButtonStyle))
                {
                    timer.Reset();
                    ApplyTimerChanges(property);
                }
                GUI.enabled = true;
            }
            else if (timer.Current == TimerState.Run)
            {
                // Run 상태
                
                Rect pauseButtonRect = new Rect(progressRect.x + progressRect.width + progressSpacing + inputWidth + buttonSpacing, progressRect.y, buttonSize, buttonSize);
                Rect stopButtonRect = new Rect(progressRect.x + progressRect.width + progressSpacing + inputWidth + buttonSpacing + buttonSize + buttonSpacing, progressRect.y, buttonSize, buttonSize);

                GUI.enabled = false;
                EditorGUI.FloatField(new Rect(progressRect.x + progressRect.width + progressSpacing, progressRect.y, inputWidth, progressRect.height), timer.Duration);
                GUI.enabled = true;

                if (GUI.Button(pauseButtonRect, EditorGUIUtility.IconContent("d_PauseButton"), compactButtonStyle))
                {
                    timer.Pause();
                    ApplyTimerChanges(property);
                }

                if (GUI.Button(stopButtonRect, EditorGUIUtility.IconContent("d_PreMatQuad"), compactButtonStyle))
                {
                    timer.Stop();
                    ApplyTimerChanges(property);
                }
            }
            else if (timer.Current == TimerState.Pause)
            {
                // Pause 상태
                
                Rect resumeButtonRect = new Rect(progressRect.x + progressRect.width + progressSpacing + inputWidth + buttonSpacing, progressRect.y, buttonSize, buttonSize);
                Rect stopButtonRect = new Rect(progressRect.x + progressRect.width + progressSpacing + inputWidth + buttonSpacing + buttonSize + buttonSpacing, progressRect.y, buttonSize, buttonSize);

                GUI.enabled = false;
                EditorGUI.FloatField(new Rect(progressRect.x + progressRect.width + progressSpacing, progressRect.y, inputWidth, progressRect.height), timer.Duration);
                GUI.enabled = true;

                if (GUI.Button(resumeButtonRect, EditorGUIUtility.IconContent("d_PlayButton"), compactButtonStyle))
                {
                    timer.Resume();
                    ApplyTimerChanges(property);
                }

                if (GUI.Button(stopButtonRect, EditorGUIUtility.IconContent("d_PreMatQuad"), compactButtonStyle))
                {
                    timer.Stop();
                    ApplyTimerChanges(property);
                }
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight + 26; // 정보 라인 + 프로그레스 바 + 여백
        }

        private void DrawStateBox(Rect stateRect, TimerState state)
        {
            // 상태 박스 배경
            GUI.Box(stateRect, "", GUI.skin.box);
            
            // 상태 텍스트
            string stateText = state.ToString();
            GUIStyle centerStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = GetStateTextColor(state) }
            };
            
            EditorGUI.LabelField(stateRect, stateText, centerStyle);
        }

        private void DrawProgressBar(Rect progressRect, Timer timer)
        {
            // 배경
            GUI.Box(progressRect, "", GUI.skin.box);
            
            // 프로그레스 계산
            float progress = 0f;

            if (timer.Duration > 0f)
            {
                progress = timer.ElapsedTime / timer.Duration;
                progress = Mathf.Clamp01(progress);
            }
            
            // 프로그레스 바
            Rect fillRect = new Rect(progressRect.x + 2, progressRect.y + 2, (progressRect.width - 4) * progress, progressRect.height - 4);
            Color progressColor = EditorGUIUtility.isProSkin ? new Color(0.3f, 0.6f, 1f, 0.8f) : new Color(0.2f, 0.4f, 0.8f, 0.8f);
            EditorGUI.DrawRect(fillRect, progressColor);
            
            // 시간 텍스트 (중앙)
            string timeText = $"{timer.ElapsedTime:F2} / {timer.Duration:F2}";
            GUIStyle centerStyle = new GUIStyle(EditorStyles.label)
            {
                alignment = TextAnchor.MiddleCenter,
                normal = { textColor = Color.white }
            };
            
            EditorGUI.DropShadowLabel(progressRect, timeText, centerStyle);
        }

        private Color GetStateTextColor(TimerState state)
        {
            switch (state)
            {
                case TimerState.Ready:
                    return Color.gray;
                case TimerState.Run:
                    return Color.green;
                case TimerState.Pause:
                    return Color.yellow;
                default:
                    return Color.white;
            }
        }
    }

#endif

}
