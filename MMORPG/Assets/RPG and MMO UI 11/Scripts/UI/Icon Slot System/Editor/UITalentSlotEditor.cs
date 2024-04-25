using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UITalentSlot), true)]
	public class UITalentSlotEditor : UISlotBaseEditor {
		
		private SerializedProperty m_PointsTextProperty;
		private SerializedProperty m_pointsMinColorProperty;
		private SerializedProperty m_pointsMaxColorProperty;
		private SerializedProperty m_pointsActiveColorProperty;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_PointsTextProperty = this.serializedObject.FindProperty("m_PointsText");
			this.m_pointsMinColorProperty = this.serializedObject.FindProperty("m_pointsMinColor");
			this.m_pointsMaxColorProperty = this.serializedObject.FindProperty("m_pointsMaxColor");
			this.m_pointsActiveColorProperty = this.serializedObject.FindProperty("m_pointsActiveColor");
		}
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.Separator();
			this.DrawPointsProperties();
			this.serializedObject.ApplyModifiedProperties();
			EditorGUILayout.Separator();
			base.OnInspectorGUI();
			EditorGUILayout.Separator();
		}
		
		protected void DrawPointsProperties()
		{
			EditorGUILayout.LabelField("Points Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_PointsTextProperty, new GUIContent("Text Component"));
			EditorGUILayout.PropertyField(this.m_pointsMinColorProperty, new GUIContent("Minimum Color"));
			EditorGUILayout.PropertyField(this.m_pointsMaxColorProperty, new GUIContent("Maximum Color"));
			EditorGUILayout.PropertyField(this.m_pointsActiveColorProperty, new GUIContent("Active Color"));
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
	}
}
