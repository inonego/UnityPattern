using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
 
namespace inonego
{
    public class ReadOnlyAttribute : PropertyAttribute { }

    #if UNITY_EDITOR

    [CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
    public class ReadOnlyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var previousGUIEnabled = GUI.enabled;

            GUI.enabled = false;

            EditorGUI.PropertyField(position, property, label);

            GUI.enabled = previousGUIEnabled;
        }
    }

    #endif
}
