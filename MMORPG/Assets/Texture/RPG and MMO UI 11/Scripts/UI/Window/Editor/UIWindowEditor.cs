using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(DuloGames.UI.UIWindow))]
	public class UIWindowEditor : Editor {
		
		private SerializedProperty m_WindowIdProperty;
		private SerializedProperty m_CustomWindowIdProperty;
		private SerializedProperty m_StartingStateProperty;
		private SerializedProperty m_EscapeKeyActionProperty;
        private SerializedProperty m_UseBlackOverlayProperty;
        private SerializedProperty m_FocusAllowReparent;
        private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_TransitionEasingProperty;
		private SerializedProperty m_TransitionDurationProperty;
		private SerializedProperty onTransitionBeginProperty;
		private SerializedProperty onTransitionCompleteProperty;
		
		protected virtual void OnEnable()
		{
			this.m_WindowIdProperty = this.serializedObject.FindProperty("m_WindowId");
			this.m_CustomWindowIdProperty = this.serializedObject.FindProperty("m_CustomWindowId");
			this.m_StartingStateProperty = this.serializedObject.FindProperty("m_StartingState");
			this.m_EscapeKeyActionProperty = this.serializedObject.FindProperty("m_EscapeKeyAction");
            this.m_UseBlackOverlayProperty = this.serializedObject.FindProperty("m_UseBlackOverlay");
            this.m_FocusAllowReparent = this.serializedObject.FindProperty("m_FocusAllowReparent");
			this.m_TransitionProperty = this.serializedObject.FindProperty("m_Transition");
			this.m_TransitionEasingProperty = this.serializedObject.FindProperty("m_TransitionEasing");
			this.m_TransitionDurationProperty = this.serializedObject.FindProperty("m_TransitionDuration");
			this.onTransitionBeginProperty = this.serializedObject.FindProperty("onTransitionBegin");
			this.onTransitionCompleteProperty = this.serializedObject.FindProperty("onTransitionComplete");
		}
		
		public override void OnInspectorGUI()
		{
			serializedObject.Update();
			EditorGUILayout.Separator();
			this.DrawGeneralProperties();
			EditorGUILayout.Separator();
            this.DrawFocusProperties();
            EditorGUILayout.Separator();
            this.DrawTransitionProperties();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.onTransitionBeginProperty, new GUIContent("On Transition Begin"), true);
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.onTransitionCompleteProperty, new GUIContent("On Transition Complete"), true);
			
			serializedObject.ApplyModifiedProperties();
		}
		
		protected void DrawGeneralProperties()
		{
			UIWindowID id = (UIWindowID)this.m_WindowIdProperty.enumValueIndex;
			
			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_WindowIdProperty, new GUIContent("ID"));
			if (id == UIWindowID.Custom)
			{
				EditorGUILayout.PropertyField(this.m_CustomWindowIdProperty, new GUIContent("Custom ID"));
			}
            EditorGUI.BeginChangeCheck();
            EditorGUILayout.PropertyField(this.m_StartingStateProperty, new GUIContent("Starting State"));
            if (EditorGUI.EndChangeCheck())
            {
                foreach (Object target in this.targets)
                    (target as UIWindow).ApplyVisualState((UIWindow.VisualState)this.m_StartingStateProperty.enumValueIndex);
            }
            EditorGUILayout.PropertyField(this.m_EscapeKeyActionProperty, new GUIContent("Escape Key Action"));
            EditorGUILayout.PropertyField(this.m_UseBlackOverlayProperty, new GUIContent("Use Black Overlay"));
            
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}

        protected void DrawFocusProperties()
        {
            EditorGUILayout.LabelField("Focus Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

            EditorGUILayout.PropertyField(this.m_FocusAllowReparent, new GUIContent("Allow Re-parenting"));
            
            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }

        protected void DrawTransitionProperties()
		{
			EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUIContent("Transition"));

            // Get the transition
            DuloGames.UI.UIWindow.Transition transition = (DuloGames.UI.UIWindow.Transition)this.m_TransitionProperty.enumValueIndex;
			
			if (transition == DuloGames.UI.UIWindow.Transition.Fade)
			{
				EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
				EditorGUILayout.PropertyField(this.m_TransitionEasingProperty, new GUIContent("Easing"));
				EditorGUILayout.PropertyField(this.m_TransitionDurationProperty, new GUIContent("Duration"));
				EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
	}
}
