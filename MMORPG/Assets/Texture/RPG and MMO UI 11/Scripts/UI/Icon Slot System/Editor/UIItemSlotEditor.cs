using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UIItemSlot), true)]
	public class UIItemSlotEditor : UISlotBaseEditor {

        private SerializedProperty m_SlotGroupProperty;
        private SerializedProperty m_IDProperty;
        private SerializedProperty onRightClickProperty;
        private SerializedProperty onDoubleClickProperty;
        private SerializedProperty onAssignProperty;
        private SerializedProperty onAssignWithSourceProperty;
        private SerializedProperty onUnassignProperty;
		
		protected override void OnEnable()
		{
			base.OnEnable();
            this.m_SlotGroupProperty = this.serializedObject.FindProperty("m_SlotGroup");
            this.m_IDProperty = this.serializedObject.FindProperty("m_ID");
            this.onRightClickProperty = this.serializedObject.FindProperty("onRightClick");
            this.onDoubleClickProperty = this.serializedObject.FindProperty("onDoubleClick");
			this.onAssignProperty = this.serializedObject.FindProperty("onAssign");
            this.onAssignWithSourceProperty = this.serializedObject.FindProperty("onAssignWithSource");
            this.onUnassignProperty = this.serializedObject.FindProperty("onUnassign");
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
            EditorGUILayout.PropertyField(this.onRightClickProperty, new GUIContent("On Right Click"), true);
            EditorGUILayout.PropertyField(this.onDoubleClickProperty, new GUIContent("On Double Click"), true);
            EditorGUILayout.PropertyField(this.onAssignProperty, new GUIContent("On Assign"), true);
            EditorGUILayout.PropertyField(this.onAssignWithSourceProperty, new GUIContent("On Assign With Source"), true);
            EditorGUILayout.PropertyField(this.onUnassignProperty, new GUIContent("On Unassign"), true);
			this.serializedObject.ApplyModifiedProperties();
		}
	}
}
