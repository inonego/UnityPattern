using UnityEngine;
using UnityEditor;

using System;
using System.Collections.Generic;

#if UNITY_EDITOR

namespace inonego.editor
{
    using inonego;

[CustomPropertyDrawer(typeof(Timer))]
public class TimerDrawer : PropertyDrawer
{
    private const float Spacing = 5f;
    private bool showDetails = true;
    private float inputTargetTime = 0f;
    private float inputCurrentTime = 0f;

    private (string icon, string tooltip, Action action) GetButtonInfo(Timer timer)
    {
        return timer.Current switch
        {
            Timer.State.Started =>  ( "PauseButton", "일시정지", timer.Pause  ),
            Timer.State.Paused =>   ( "PlayButton",  "재개",     timer.Resume ),
            Timer.State.Stopped =>  ( "PlayButton",  "시작",     timer.Start  ),

            _ => (string.Empty, string.Empty, null)
        };
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        Timer timer = (Timer)fieldInfo.GetValue(property.serializedObject.targetObject);
        
        // Foldout 헤더와 시간 정보
        var rect = position;

        // 높이 설정
        rect.height = EditorGUIUtility.singleLineHeight;

        // 시간 정보 텍스트
        var timeText = $"{timer.Time.ElapsedTime:F1} / {timer.Time.Target:F1}";

        var timeContent = new GUIContent(timeText);
        var timeWidth = GUI.skin.label.CalcSize(timeContent).x;

        var foldoutRect = rect;
        foldoutRect.width -= timeWidth + 5f;
        
        var timeRect = rect;
        timeRect.x = rect.width - timeWidth + 15f;
        timeRect.width = timeWidth;

        showDetails = EditorGUI.Foldout(foldoutRect, showDetails, label);
        
        if (!showDetails)
        {
            EditorGUI.LabelField(timeRect, timeText);
        }
        else
        {
            #region 프로그레스 바

            // 들여쓰기를 위한 조정
            var indent = EditorGUI.IndentedRect(position);
            rect = indent;
            rect.y += EditorGUIUtility.singleLineHeight + Spacing;
            rect.height = EditorGUIUtility.singleLineHeight;
            
            // 프로그레스 바 (실제 Timer의 값을 표시)
            float progress = (float)(timer.Time.ElapsedTime01);
            EditorGUI.ProgressBar(rect, progress, timeText);

            #endregion

            // 컨트롤 버튼들
            rect.y += EditorGUIUtility.singleLineHeight + Spacing;
            
            var controlButtonStyle = new GUIStyle(GUI.skin.button);
            controlButtonStyle.margin = new RectOffset(0, 0, 0, 0);
            controlButtonStyle.fixedHeight = 20f;
            
            var buttonRect = rect;
            buttonRect.width = rect.width / 2;
            buttonRect.height = 20f;

        #region 재생 버튼

            var (icon, tooltip, action) = GetButtonInfo(timer);

            // 왼쪽 버튼 (재생/일시정지/재개)
            var leftContent = EditorGUIUtility.IconContent(icon);
            
            leftContent.tooltip = tooltip;

            if (GUI.Button(buttonRect, leftContent, controlButtonStyle))
            {
                action?.Invoke();
            }

        #endregion

        #region 정지 버튼

            // 오른쪽 버튼 (정지)
            buttonRect.x += buttonRect.width;
            var stopContent = EditorGUIUtility.IconContent("PreMatQuad");
            stopContent.tooltip = "정지";
            if (GUI.Button(buttonRect, stopContent, controlButtonStyle))
            {
                timer.Stop();
            }

            // Time 데이터 설정 영역
            rect.y += EditorGUIUtility.singleLineHeight + Spacing;
            
            // 한 줄에 Current와 Target 모두 표시
            var halfWidth = (rect.width - Spacing) / 2;
            
        #endregion

        #region Current

            // Current (왼쪽)
            var currentRect = rect;
            currentRect.width = halfWidth;
            var currentLabelRect = currentRect;
            currentLabelRect.width = 30f;  // 라벨 너비 축소
            EditorGUI.LabelField(currentLabelRect, "현재");

            var currentInputRect = currentRect;
            currentInputRect.x += 30f + Spacing;
            currentInputRect.width = halfWidth - 72f;  // 입력 필드 너비 조정
            inputCurrentTime = EditorGUI.FloatField(currentInputRect, inputCurrentTime);

            var currentButtonRect = currentRect;
            currentButtonRect.x += halfWidth - 35f;
            currentButtonRect.width = 35f;  // 버튼 너비 살짝 증가
            if (GUI.Button(currentButtonRect, "설정"))
            {
                timer.Time.Current = inputCurrentTime;
            }

        #endregion

        #region Target

            // Target (오른쪽)
            var targetRect = rect;
            targetRect.x += halfWidth + Spacing;
            targetRect.width = halfWidth;

            var targetLabelRect = targetRect;
            targetLabelRect.width = 30f;  // 라벨 너비 축소
            EditorGUI.LabelField(targetLabelRect, "목표");

            var targetInputRect = targetRect;
            targetInputRect.x += 30f + Spacing;
            targetInputRect.width = halfWidth - 72f;  // 입력 필드 너비 조정
            inputTargetTime = EditorGUI.FloatField(targetInputRect, inputTargetTime);

            var targetButtonRect = targetRect;
            targetButtonRect.x += halfWidth - 35f;
            targetButtonRect.width = 35f;  // 버튼 너비 살짝 증가
            if (GUI.Button(targetButtonRect, "설정"))
            {
                timer.Time.Target = inputTargetTime;
            }

        #endregion
        }

        if (GUI.changed)
        {
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (!showDetails)
            return EditorGUIUtility.singleLineHeight;
            
        return EditorGUIUtility.singleLineHeight * 5;  // 전체 높이 조정
    }
}

}

#endif