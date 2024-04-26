using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(UIEffectTransition))]
    public class UIEffectTransitionEditor : Editor
    {
        private SerializedProperty m_TargetEffectProperty;
        private SerializedProperty m_NormalColorProperty;
        private SerializedProperty m_HighlightedColorProperty;
        private SerializedProperty m_SelectedColorProperty;
        private SerializedProperty m_PressedColorProperty;
        private SerializedProperty m_DurationProperty;
        private SerializedProperty m_UseToggleProperty;
        private SerializedProperty m_TargetToggleProperty;
        private SerializedProperty m_ActiveColorProperty;

        protected void OnEnable()
        {
            this.m_TargetEffectProperty = this.serializedObject.FindProperty("m_TargetEffect");
            this.m_NormalColorProperty = this.serializedObject.FindProperty("m_NormalColor");
            this.m_HighlightedColorProperty = this.serializedObject.FindProperty("m_HighlightedColor");
            this.m_SelectedColorProperty = this.serializedObject.FindProperty("m_SelectedColor");
            this.m_PressedColorProperty = this.serializedObject.FindProperty("m_PressedColor");
            this.m_DurationProperty = this.serializedObject.FindProperty("m_Duration");
            this.m_UseToggleProperty = this.serializedObject.FindProperty("m_UseToggle");
            this.m_TargetToggleProperty = this.serializedObject.FindProperty("m_TargetToggle");
            this.m_ActiveColorProperty = this.serializedObject.FindProperty("m_ActiveColor");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            BaseMeshEffect effect = this.m_TargetEffectProperty.objectReferenceValue as BaseMeshEffect;

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(this.m_TargetEffectProperty, new GUIContent("Target Effect"));
            EditorGUI.indentLevel++;
            
            if (effect == null || ((effect is Shadow) == false && (effect is Outline) == false))
            {
                EditorGUILayout.HelpBox("You must have Shadow or Outline effect target in order to use this transition.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.PropertyField(this.m_NormalColorProperty, true);
                EditorGUILayout.PropertyField(this.m_HighlightedColorProperty, true);
                EditorGUILayout.PropertyField(this.m_SelectedColorProperty, true);
                EditorGUILayout.PropertyField(this.m_PressedColorProperty, true);
                EditorGUILayout.PropertyField(this.m_DurationProperty, true);
            }

            EditorGUILayout.PropertyField(this.m_UseToggleProperty, true);

            if (this.m_UseToggleProperty.boolValue)
            {
                EditorGUILayout.PropertyField(this.m_TargetToggleProperty, true);
                EditorGUILayout.PropertyField(this.m_ActiveColorProperty, true);
                
            }

            this.serializedObject.ApplyModifiedProperties();
        }
    }
}
