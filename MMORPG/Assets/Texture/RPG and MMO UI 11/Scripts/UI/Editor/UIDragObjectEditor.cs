using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UIDragObject), true)]
    public class UIDragObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Target"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Horizontal"));
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Vertical"));

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Inertia", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_Inertia"), new GUIContent("Enable"));
            if (this.serializedObject.FindProperty("m_Inertia").boolValue)
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_DampeningRate"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_InertiaRounding"));
            }
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Constrain Within Canvas", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_ConstrainWithinCanvas"), new GUIContent("Enable"));
            if (this.serializedObject.FindProperty("m_ConstrainWithinCanvas").boolValue)
            {
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_ConstrainDrag"), new GUIContent("Constrain Drag"));
                EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_ConstrainInertia"), new GUIContent("Constrain Inertia"));
            }
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onBeginDrag"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onEndDrag"), true);
            EditorGUILayout.PropertyField(this.serializedObject.FindProperty("onDrag"), true);

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
