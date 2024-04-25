using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UIEquipSlot), true)]
	public class UIEquipSlotEditor : UISlotBaseEditor {
		
		private SerializedProperty m_EquipTypeProperty;
		private SerializedProperty onAssignProperty;
        private SerializedProperty onAssignWithSourceProperty;
        private SerializedProperty onUnassignProperty;
		
		protected override void OnEnable()
		{
			base.OnEnable();
			this.m_EquipTypeProperty = this.serializedObject.FindProperty("m_EquipType");
			this.onAssignProperty = this.serializedObject.FindProperty("onAssign");
            this.onAssignWithSourceProperty = this.serializedObject.FindProperty("onAssignWithSource");
            this.onUnassignProperty = this.serializedObject.FindProperty("onUnassign");
		}
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.m_EquipTypeProperty, new GUIContent("Equip Type"));
			EditorGUILayout.Separator();
			this.serializedObject.ApplyModifiedProperties();
			
			base.OnInspectorGUI();
			
			EditorGUILayout.Separator();
			
			this.serializedObject.Update();
			EditorGUILayout.PropertyField(this.onAssignProperty, new GUIContent("On Assign"), true);
            EditorGUILayout.PropertyField(this.onAssignWithSourceProperty, new GUIContent("On Assign With Source"), true);
            EditorGUILayout.PropertyField(this.onUnassignProperty, new GUIContent("On Unassign"), true);
			this.serializedObject.ApplyModifiedProperties();
			
		}
	}
}
