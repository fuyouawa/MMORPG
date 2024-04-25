using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UISpellSlot), true)]
	public class UISpellSlotEditor : UISlotBaseEditor {
		
		private SerializedProperty m_SlotGroupProperty;
		private SerializedProperty m_IDProperty;
		private SerializedProperty onAssignProperty;
		private SerializedProperty onUnassignProperty;
		private SerializedProperty onClickProperty;
			
		protected override void OnEnable()
		{
			base.OnEnable();
			
			this.m_SlotGroupProperty = this.serializedObject.FindProperty("m_SlotGroup");
			this.m_IDProperty = this.serializedObject.FindProperty("m_ID");
			this.onAssignProperty = this.serializedObject.FindProperty("onAssign");
			this.onUnassignProperty = this.serializedObject.FindProperty("onUnassign");
			this.onClickProperty = this.serializedObject.FindProperty("onClick");
		}
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.m_SlotGroupProperty, new GUIContent("Slot Group"));
			EditorGUILayout.PropertyField(m_IDProperty, new GUIContent("Slot ID"));
            EditorGUILayout.Separator();
            this.serializedObject.ApplyModifiedProperties();
			
			base.OnInspectorGUI();

            EditorGUILayout.Separator();

            this.serializedObject.Update();
			EditorGUILayout.PropertyField(this.onAssignProperty, new GUIContent("On Assign"), true);
			EditorGUILayout.PropertyField(this.onUnassignProperty, new GUIContent("On Unassign"), true);
			EditorGUILayout.PropertyField(this.onClickProperty, new GUIContent("On Click"), true);
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}
