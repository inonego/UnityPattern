using UnityEditor;
using UnityEngine;

namespace inonego
{

[CustomEditor(typeof(Health))]
public class HealthEditor : Editor
{
    private int newMaxHP = 0;
    private int newHP = 0;
    private int healValue = 0;
    private int damageValue = 0;

    public override void OnInspectorGUI()
    {
        Health health = (Health)target;

        // 상태 관리 섹션
        EditorGUILayout.LabelField("상태 관리", EditorStyles.boldLabel);
        EditorGUILayout.LabelField("현재 상태:", health.Current.ToString());
        health.AliveOnAwake = EditorGUILayout.Toggle("시작 시 살아있는 상태", health.AliveOnAwake);
        health.DestroyOnDead = EditorGUILayout.Toggle("죽을 때 오브젝트 제거", health.DestroyOnDead);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("살아있는 상태로 설정"))
        {
            health.SetAlive();
        }
        if (GUILayout.Button("죽은 상태로 설정"))
        {
            health.SetDead();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        // 체력 관리 섹션 - 체력 바 상단에 표시
        EditorGUILayout.LabelField("체력 관리", EditorStyles.boldLabel);
        float healthRatio = health.MaxHP > 0 ? (float)health.HP / health.MaxHP : 0;
        Rect healthBarRect = EditorGUILayout.GetControlRect(false, 20);
        EditorGUI.ProgressBar(healthBarRect, healthRatio, $"{health.HP} / {health.MaxHP}");
        EditorGUILayout.Space();
        
        // 최대 체력 설정
        EditorGUILayout.BeginHorizontal();
        newMaxHP = EditorGUILayout.IntField("최대 체력", newMaxHP);
        if (GUILayout.Button("설정", GUILayout.Width(50)))
        {
            health.SetMaxHP(newMaxHP);
        }
        EditorGUILayout.EndHorizontal();
        
        // 현재 체력 설정
        EditorGUILayout.BeginHorizontal();
        newHP = EditorGUILayout.IntField("현재 체력", newHP);
        if (GUILayout.Button("설정", GUILayout.Width(50)))
        {
            health.SetHP(newHP);
        }
        EditorGUILayout.EndHorizontal();
        
        // 액션 섹션 - 액션 발생
        EditorGUILayout.LabelField("액션", EditorStyles.boldLabel);

        // 힐 적용
        EditorGUILayout.BeginHorizontal();
        healValue = EditorGUILayout.IntField("힐", healValue);
        if (GUILayout.Button("적용", GUILayout.Width(50)))
        {
            health.ApplyHeal(healValue);
        }
        EditorGUILayout.EndHorizontal();

        // 데미지 적용
        EditorGUILayout.BeginHorizontal();
        damageValue = EditorGUILayout.IntField("데미지", damageValue);
        if (GUILayout.Button("적용", GUILayout.Width(50)))
        {
            health.ApplyDamage(damageValue);
        }
        EditorGUILayout.EndHorizontal();

        if (GUI.changed)
        {
            EditorUtility.SetDirty(target);
        }
    }
}

}