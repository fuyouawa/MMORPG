using UnityEngine;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CustomEditor(typeof(UITooltip), true)]
	public class UITooltipEditor : Editor {
		
		private SerializedProperty m_DefaultWidthProperty;
        private SerializedProperty m_AnchoringProperty;
		private SerializedProperty m_AnchorGraphicProperty;
		private SerializedProperty m_AnchorGraphicOffsetProperty;
		private SerializedProperty m_followMouseProperty;
		private SerializedProperty m_OffsetProperty;
		private SerializedProperty m_AnchoredOffsetProperty;
		private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_TransitionEasingProperty;
		private SerializedProperty m_TransitionDurationProperty;
        //private SerializedProperty m_OnAnchorEventProperty;

        protected virtual void OnEnable()
		{
			this.m_DefaultWidthProperty = this.serializedObject.FindProperty("m_DefaultWidth");
            this.m_AnchoringProperty = this.serializedObject.FindProperty("m_Anchoring");
            this.m_AnchorGraphicProperty = this.serializedObject.FindProperty("m_AnchorGraphic");
			this.m_AnchorGraphicOffsetProperty = this.serializedObject.FindProperty("m_AnchorGraphicOffset");
			this.m_followMouseProperty = this.serializedObject.FindProperty("m_followMouse");
			this.m_OffsetProperty = this.serializedObject.FindProperty("m_Offset");
			this.m_AnchoredOffsetProperty = this.serializedObject.FindProperty("m_AnchoredOffset");
			this.m_TransitionProperty = this.serializedObject.FindProperty("m_Transition");
			this.m_TransitionEasingProperty = this.serializedObject.FindProperty("m_TransitionEasing");
			this.m_TransitionDurationProperty = this.serializedObject.FindProperty("m_TransitionDuration");
            //this.m_OnAnchorEventProperty = this.serializedObject.FindProperty("onAnchorEvent");
        }
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			EditorGUILayout.Separator();
			this.DrawGeneralProperties();
			EditorGUILayout.Separator();
			this.DrawAnchorProperties();
			EditorGUILayout.Separator();
			this.DrawTransitionProperties();
            EditorGUILayout.Separator();
            //EditorGUILayout.PropertyField(this.m_OnAnchorEventProperty, new GUIContent("On Anchor Event"), true);
            //EditorGUILayout.Separator();
            this.serializedObject.ApplyModifiedProperties();
		}
		
		protected void DrawGeneralProperties()
		{
			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUIUtility.labelWidth = 150f;
			EditorGUILayout.PropertyField(this.m_followMouseProperty, new GUIContent("Follow Mouse"));
			EditorGUILayout.PropertyField(this.m_OffsetProperty, new GUIContent("Offset"));
			EditorGUILayout.PropertyField(this.m_AnchoredOffsetProperty, new GUIContent("Anchored Offset"));
			EditorGUILayout.PropertyField(this.m_DefaultWidthProperty, new GUIContent("Default Width"));
            EditorGUILayout.PropertyField(this.m_AnchoringProperty, new GUIContent("Anchoring"));
            EditorGUIUtility.labelWidth = 120f;
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected void DrawTransitionProperties()
		{
			EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUIContent("Transition"));
			
			UITooltip.Transition transition = (UITooltip.Transition)this.m_TransitionProperty.enumValueIndex;
			
			if (transition != UITooltip.Transition.None)
			{
				EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
				
				if (transition == UITooltip.Transition.Fade)
				{
					EditorGUILayout.PropertyField(this.m_TransitionEasingProperty, new GUIContent("Easing"));
					EditorGUILayout.PropertyField(this.m_TransitionDurationProperty, new GUIContent("Duration"));
				}
				
				EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected void DrawAnchorProperties()
		{
			EditorGUILayout.LabelField("Anchor Graphic Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_AnchorGraphicProperty, new GUIContent("Graphic"));
			EditorGUILayout.PropertyField(this.m_AnchorGraphicOffsetProperty, new GUIContent("Offset"));
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
	}
}
