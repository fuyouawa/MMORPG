using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UIHighlightTransition))]
	public class UIHighlightTransitionEditor : Editor {
		
		private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_TargetGraphicProperty;
		private SerializedProperty m_TargetGameObjectProperty;
		private SerializedProperty m_NormalColorProperty;
		private SerializedProperty m_HighlightedColorProperty;
		private SerializedProperty m_SelectedColorProperty;
        private SerializedProperty m_PressedColorProperty;
        private SerializedProperty m_DurationProperty;
		private SerializedProperty m_ColorMultiplierProperty;
		private SerializedProperty m_HighlightedSpriteProperty;
		private SerializedProperty m_SelectedSpriteProperty;
        private SerializedProperty m_PressedSpriteProperty;
        private SerializedProperty m_NormalTriggerProperty;
		private SerializedProperty m_HighlightedTriggerProperty;
		private SerializedProperty m_SelectedTriggerProperty;
        private SerializedProperty m_PressedTriggerProperty;
        private SerializedProperty m_UseToggleProperty;
        private SerializedProperty m_TargetToggleProperty;
        private SerializedProperty m_ActiveColorProperty;
        private SerializedProperty m_ActiveSpriteProperty;
        private SerializedProperty m_ActiveBoolProperty;
        private SerializedProperty m_TargetCanvasGroupProperty;
        private SerializedProperty m_NormalAlphaProperty;
        private SerializedProperty m_HighlightedAlphaProperty;
        private SerializedProperty m_SelectedAlphaProperty;
        private SerializedProperty m_PressedAlphaProperty;
        private SerializedProperty m_ActiveAlphaProperty;

        protected void OnEnable()
		{
			this.m_TransitionProperty = this.serializedObject.FindProperty("m_Transition");
			this.m_TargetGraphicProperty = this.serializedObject.FindProperty("m_TargetGraphic");
			this.m_TargetGameObjectProperty = this.serializedObject.FindProperty("m_TargetGameObject");
			this.m_NormalColorProperty = this.serializedObject.FindProperty("m_NormalColor");
			this.m_HighlightedColorProperty = this.serializedObject.FindProperty("m_HighlightedColor");
			this.m_SelectedColorProperty = this.serializedObject.FindProperty("m_SelectedColor");
            this.m_PressedColorProperty = this.serializedObject.FindProperty("m_PressedColor");
			this.m_DurationProperty = this.serializedObject.FindProperty("m_Duration");
			this.m_ColorMultiplierProperty = this.serializedObject.FindProperty("m_ColorMultiplier");
			this.m_HighlightedSpriteProperty = this.serializedObject.FindProperty("m_HighlightedSprite");
			this.m_SelectedSpriteProperty = this.serializedObject.FindProperty("m_SelectedSprite");
            this.m_PressedSpriteProperty = this.serializedObject.FindProperty("m_PressedSprite");
            this.m_NormalTriggerProperty = this.serializedObject.FindProperty("m_NormalTrigger");
			this.m_HighlightedTriggerProperty = this.serializedObject.FindProperty("m_HighlightedTrigger");
			this.m_SelectedTriggerProperty = this.serializedObject.FindProperty("m_SelectedTrigger");
            this.m_PressedTriggerProperty = this.serializedObject.FindProperty("m_PressedTrigger");
            this.m_UseToggleProperty = this.serializedObject.FindProperty("m_UseToggle");
            this.m_TargetToggleProperty = this.serializedObject.FindProperty("m_TargetToggle");
            this.m_ActiveColorProperty = this.serializedObject.FindProperty("m_ActiveColor");
            this.m_ActiveSpriteProperty = this.serializedObject.FindProperty("m_ActiveSprite");
            this.m_ActiveBoolProperty = this.serializedObject.FindProperty("m_ActiveBool");
            this.m_TargetCanvasGroupProperty = this.serializedObject.FindProperty("m_TargetCanvasGroup");
            this.m_NormalAlphaProperty = this.serializedObject.FindProperty("m_NormalAlpha");
            this.m_HighlightedAlphaProperty = this.serializedObject.FindProperty("m_HighlightedAlpha");
            this.m_SelectedAlphaProperty = this.serializedObject.FindProperty("m_SelectedAlpha");
            this.m_PressedAlphaProperty = this.serializedObject.FindProperty("m_PressedAlpha");
            this.m_ActiveAlphaProperty = this.serializedObject.FindProperty("m_ActiveAlpha");
        }
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();

            UIHighlightTransition.Transition transition = (UIHighlightTransition.Transition)this.m_TransitionProperty.enumValueIndex;
			Graphic graphic = this.m_TargetGraphicProperty.objectReferenceValue as Graphic;
			GameObject targetGameObject = this.m_TargetGameObjectProperty.objectReferenceValue as GameObject;
            CanvasGroup targetCanvasGroup = this.m_TargetCanvasGroupProperty.objectReferenceValue as CanvasGroup;

            EditorGUILayout.Space();
			EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUIContent("Transition"));
			EditorGUI.indentLevel++;
			
			// Check if the transition requires a graphic
			if (transition == UIHighlightTransition.Transition.ColorTint || transition == UIHighlightTransition.Transition.SpriteSwap || transition == UIHighlightTransition.Transition.TextColor)
			{
				EditorGUILayout.PropertyField(this.m_TargetGraphicProperty, new GUIContent("Target Graphic"));
				
				if (transition == UIHighlightTransition.Transition.ColorTint)
				{
					if (graphic == null)
					{
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Info);
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.m_NormalColorProperty, true);
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(this.m_NormalColorProperty.colorValue);
						
						EditorGUILayout.PropertyField(this.m_HighlightedColorProperty, true);
						EditorGUILayout.PropertyField(this.m_SelectedColorProperty, true);
                        EditorGUILayout.PropertyField(this.m_PressedColorProperty, true);
						EditorGUILayout.PropertyField(this.m_ColorMultiplierProperty, true);
						EditorGUILayout.PropertyField(this.m_DurationProperty, true);
					}
				}
                else if (transition == UIHighlightTransition.Transition.TextColor)
                {
                    if (graphic == null || (graphic is Text) == false)
                    {
                        EditorGUILayout.HelpBox("You must have a Text target in order to use a text color transition.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(this.m_NormalColorProperty, true);
                        if (EditorGUI.EndChangeCheck())
                            (graphic as Text).color = this.m_NormalColorProperty.colorValue;

                        EditorGUILayout.PropertyField(this.m_HighlightedColorProperty, true);
                        EditorGUILayout.PropertyField(this.m_SelectedColorProperty, true);
                        EditorGUILayout.PropertyField(this.m_PressedColorProperty, true);
                        EditorGUILayout.PropertyField(this.m_DurationProperty, true);
                    }
                }
				else if (transition == UIHighlightTransition.Transition.SpriteSwap)
				{
					if (graphic == null || (graphic is Image) == false)
					{
						EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					}
					else
					{
						EditorGUILayout.PropertyField(this.m_HighlightedSpriteProperty, true);
						EditorGUILayout.PropertyField(this.m_SelectedSpriteProperty, true);
                        EditorGUILayout.PropertyField(this.m_PressedSpriteProperty, true);
                    }
				}
			}
			else if (transition == UIHighlightTransition.Transition.Animation)
			{
				EditorGUILayout.PropertyField(this.m_TargetGameObjectProperty, new GUIContent("Target GameObject"));
				
				if (targetGameObject == null)
				{
					EditorGUILayout.HelpBox("You must have a Game Object target in order to use a animation transition.", MessageType.Info);
				}
				else
				{
					EditorGUILayout.PropertyField(this.m_NormalTriggerProperty, true);
					EditorGUILayout.PropertyField(this.m_HighlightedTriggerProperty, true);
					EditorGUILayout.PropertyField(this.m_SelectedTriggerProperty, true);
                    EditorGUILayout.PropertyField(this.m_PressedTriggerProperty, true);

                    Animator animator = targetGameObject.GetComponent<Animator>();
					
					if (animator == null || animator.runtimeAnimatorController == null)
					{
						Rect controlRect = EditorGUILayout.GetControlRect();
						controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);
						
						if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
						{
							// Generate the animator controller
							UnityEditor.Animations.AnimatorController animatorController = this.GenerateAnimatorController();
							
							if (animatorController != null)
							{
								if (animator == null)
								{
									animator = targetGameObject.AddComponent<Animator>();
								}
								UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
							}
						}
					}
				}
			}
            else if (transition == UIHighlightTransition.Transition.CanvasGroup)
            {
                EditorGUILayout.PropertyField(this.m_TargetCanvasGroupProperty, new GUIContent("Target Canvas Group"));

                if (targetCanvasGroup == null)
                {
                    EditorGUILayout.HelpBox("You must have a CanvasGroup target in order to use this transition.", MessageType.Info);
                }
                else
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(this.m_NormalAlphaProperty, true);
                    if (EditorGUI.EndChangeCheck())
                        targetCanvasGroup.alpha = this.m_NormalAlphaProperty.floatValue;

                    EditorGUILayout.PropertyField(this.m_HighlightedAlphaProperty, true);
                    EditorGUILayout.PropertyField(this.m_SelectedAlphaProperty, true);
                    EditorGUILayout.PropertyField(this.m_PressedAlphaProperty, true);
                    EditorGUILayout.PropertyField(this.m_DurationProperty, true);
                }
            }

            EditorGUILayout.PropertyField(this.m_UseToggleProperty, true);

            if (this.m_UseToggleProperty.boolValue)
            {
                EditorGUILayout.PropertyField(this.m_TargetToggleProperty, true);

                if (transition == UIHighlightTransition.Transition.ColorTint)
                {
                    EditorGUILayout.PropertyField(this.m_ActiveColorProperty, true);
                }
                else if (transition == UIHighlightTransition.Transition.TextColor)
                {
                    EditorGUILayout.PropertyField(this.m_ActiveColorProperty, true);
                }
                else if (transition == UIHighlightTransition.Transition.SpriteSwap)
                {
                    EditorGUILayout.PropertyField(this.m_ActiveSpriteProperty, true);
                }
                else if (transition == UIHighlightTransition.Transition.Animation)
                {
                    EditorGUILayout.PropertyField(this.m_ActiveBoolProperty, true);
                }
                else if (transition == UIHighlightTransition.Transition.CanvasGroup)
                {
                    EditorGUILayout.PropertyField(this.m_ActiveAlphaProperty, true);
                }
            }

            this.serializedObject.ApplyModifiedProperties();
		}
		
		private UnityEditor.Animations.AnimatorController GenerateAnimatorController()
		{
			// Prepare the triggers list
			List<string> triggers = new List<string>();
			
			triggers.Add((!string.IsNullOrEmpty(this.m_NormalTriggerProperty.stringValue)) ? this.m_NormalTriggerProperty.stringValue : "Normal");
			triggers.Add((!string.IsNullOrEmpty(this.m_HighlightedTriggerProperty.stringValue)) ? this.m_HighlightedTriggerProperty.stringValue : "Highlighted");
			triggers.Add((!string.IsNullOrEmpty(this.m_SelectedTriggerProperty.stringValue)) ? this.m_SelectedTriggerProperty.stringValue : "Selected");
            triggers.Add((!string.IsNullOrEmpty(this.m_PressedTriggerProperty.stringValue)) ? this.m_PressedTriggerProperty.stringValue : "Pressed");

            return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers, this.m_TargetGameObjectProperty.objectReferenceValue.name);
		}
	}
}
