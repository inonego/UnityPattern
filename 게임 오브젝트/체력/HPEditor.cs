using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace inonego.editor
{

[CustomEditor(typeof(HP))]
public class HPEditor : Editor
{
    private int newMaxHP = 0;
    private int newHP = 0;
    private int healValue = 0;
    private int damageValue = 0;

    public override void OnInspectorGUI()
    {
        HP hp = (HP)target;

    #region 상태 관리 섹션

        EditorGUILayout.LabelField("상태 관리", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("현재 상태:", hp.Current.ToString());

        hp.AliveOnAwake = EditorGUILayout.Toggle("시작 시 살아있는 상태", hp.AliveOnAwake);
        hp.DestroyOnDead = EditorGUILayout.Toggle("죽을 때 오브젝트 제거", hp.DestroyOnDead);

    #endregion

    #region 상태 변경 버튼

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("살아있는 상태로 설정"))
        {
            hp.SetAlive();
        }
        if (GUILayout.Button("죽은 상태로 설정"))
        {
            hp.SetDead();
        }

        EditorGUILayout.EndHorizontal();

    #endregion

        EditorGUILayout.Space();

    #region 체력 바 표시
        
        double ratio = hp.MaxValue > 0.0 ? hp.CurrentValue / hp.MaxValue : 0.0;

        // 체력 관리 섹션 - 체력 바 상단에 표시
        EditorGUILayout.LabelField("체력 관리", EditorStyles.boldLabel);
        
        EditorGUI.ProgressBar(EditorGUILayout.GetControlRect(false, 20), (float)ratio, $"{hp.CurrentValue} / {hp.MaxValue}");

    #endregion

        EditorGUILayout.Space();
        
    #region 최대 체력 설정

        EditorGUILayout.BeginHorizontal();
        newMaxHP = EditorGUILayout.IntField("최대 체력", newMaxHP);
        if (GUILayout.Button("설정", GUILayout.Width(50)))
        {
            hp.SetMaxValue(newMaxHP);
        }
        EditorGUILayout.EndHorizontal();
        
    #endregion

    #region  현재 체력 설정

        EditorGUILayout.BeginHorizontal();
        newHP = EditorGUILayout.IntField("현재 체력", newHP);
        if (GUILayout.Button("설정", GUILayout.Width(50)))
        {
            hp.SetValue(newHP);
        }
        EditorGUILayout.EndHorizontal();

    #endregion

        // 액션 섹션 - 액션 발생
        EditorGUILayout.LabelField("액션", EditorStyles.boldLabel);

    #region 힐 적용
        EditorGUILayout.BeginHorizontal();
        healValue = EditorGUILayout.IntField("힐", healValue);
        if (GUILayout.Button("적용", GUILayout.Width(50)))
        {
            hp.ApplyHeal(healValue);
        }
        EditorGUILayout.EndHorizontal();
    #endregion

    #region 데미지 적용
        EditorGUILayout.BeginHorizontal();
        damageValue = EditorGUILayout.IntField("데미지", damageValue);
        if (GUILayout.Button("적용", GUILayout.Width(50)))
        {
            hp.ApplyDamage(damageValue);
        }
        EditorGUILayout.EndHorizontal();

    #endregion

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

}

#endif