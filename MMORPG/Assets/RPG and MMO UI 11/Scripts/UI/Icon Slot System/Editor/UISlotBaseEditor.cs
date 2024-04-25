using UnityEngine;
using UnityEngine.UI;
using DuloGames.UI;
using UnityEditor;
using System.Collections.Generic;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UISlotBase), true)]
	public class UISlotBaseEditor : Editor {
		
		private SerializedProperty m_IconGraphicProperty;
        private SerializedProperty m_CloneTargetProperty;
        private SerializedProperty m_DragAndDropEnabledProperty;
		private SerializedProperty m_IsStaticProperty;
		private SerializedProperty m_AllowThrowAwayProperty;
		private SerializedProperty m_DragKeyModifierProperty;
		private SerializedProperty m_TooltipEnabledProperty;
		private SerializedProperty m_TooltipDelayProperty;
		private SerializedProperty hoverTransitionProperty;
		private SerializedProperty hoverTargetGraphicProperty;
		private SerializedProperty hoverNormalColorProperty;
		private SerializedProperty hoverHighlightColorProperty;
		private SerializedProperty hoverTransitionDurationProperty;
		private SerializedProperty hoverOverrideSpriteProperty;
		private SerializedProperty hoverNormalTriggerProperty;
		private SerializedProperty hoverHighlightTriggerProperty;
		private SerializedProperty pressTransitionProperty;
		private SerializedProperty pressTargetGraphicProperty;
		private SerializedProperty pressNormalColorProperty;
		private SerializedProperty pressPressColorProperty;
		private SerializedProperty pressTransitionDurationProperty;
		private SerializedProperty pressTransitionInstaOutProperty;
		private SerializedProperty pressOverrideSpriteProperty;
		private SerializedProperty pressNormalTriggerProperty;
		private SerializedProperty pressPressTriggerProperty;
		private SerializedProperty pressForceHoverNormalProperty;
		
		protected virtual void OnEnable()
		{
			this.m_IconGraphicProperty = this.serializedObject.FindProperty("iconGraphic");
            this.m_CloneTargetProperty = this.serializedObject.FindProperty("m_CloneTarget");
            this.m_DragAndDropEnabledProperty = this.serializedObject.FindProperty("m_DragAndDropEnabled");
			this.m_IsStaticProperty = this.serializedObject.FindProperty("m_IsStatic");
			this.m_AllowThrowAwayProperty = this.serializedObject.FindProperty("m_AllowThrowAway");
			this.m_DragKeyModifierProperty = this.serializedObject.FindProperty("m_DragKeyModifier");
			this.m_TooltipEnabledProperty = this.serializedObject.FindProperty("m_TooltipEnabled");
			this.m_TooltipDelayProperty = this.serializedObject.FindProperty("m_TooltipDelay");
			this.hoverTransitionProperty = this.serializedObject.FindProperty("hoverTransition");
			this.hoverTargetGraphicProperty = this.serializedObject.FindProperty("hoverTargetGraphic");
			this.hoverNormalColorProperty = this.serializedObject.FindProperty("hoverNormalColor");
			this.hoverHighlightColorProperty = this.serializedObject.FindProperty("hoverHighlightColor");
			this.hoverTransitionDurationProperty = this.serializedObject.FindProperty("hoverTransitionDuration");
			this.hoverOverrideSpriteProperty = this.serializedObject.FindProperty("hoverOverrideSprite");
			this.hoverNormalTriggerProperty = this.serializedObject.FindProperty("hoverNormalTrigger");
			this.hoverHighlightTriggerProperty = this.serializedObject.FindProperty("hoverHighlightTrigger");
			this.pressTransitionProperty = this.serializedObject.FindProperty("pressTransition");
			this.pressTargetGraphicProperty = this.serializedObject.FindProperty("pressTargetGraphic");
			this.pressNormalColorProperty = this.serializedObject.FindProperty("pressNormalColor");
			this.pressPressColorProperty = this.serializedObject.FindProperty("pressPressColor");
			this.pressTransitionDurationProperty = this.serializedObject.FindProperty("pressTransitionDuration");
			this.pressTransitionInstaOutProperty = this.serializedObject.FindProperty("m_PressTransitionInstaOut");
			this.pressOverrideSpriteProperty = this.serializedObject.FindProperty("pressOverrideSprite");
			this.pressNormalTriggerProperty = this.serializedObject.FindProperty("pressNormalTrigger");
			this.pressPressTriggerProperty = this.serializedObject.FindProperty("pressPressTrigger");
			this.pressForceHoverNormalProperty = this.serializedObject.FindProperty("m_PressForceHoverNormal");
		}
		
		public override void OnInspectorGUI()
		{
			this.serializedObject.Update();
			this.DrawIconGraphicProperties();
			EditorGUILayout.Separator();
			this.DrawDragAndDropProperties();
			EditorGUILayout.Separator();
			this.DrawTooltipProperties();
			EditorGUILayout.Separator();
			this.DrawHoverProperties();
			EditorGUILayout.Separator();
			this.DrawPressProperties();
			this.serializedObject.ApplyModifiedProperties();
		}
		
		protected void DrawIconGraphicProperties()
		{
			EditorGUILayout.LabelField("Icon Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.m_IconGraphicProperty, new GUIContent("Icon Graphic"));
            EditorGUILayout.PropertyField(this.m_CloneTargetProperty, new GUIContent("Clone Target"));

            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected void DrawDragAndDropProperties()
		{
			EditorGUILayout.LabelField("Drag & Drop Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUIUtility.labelWidth = 150f;
			
			EditorGUILayout.PropertyField(this.m_DragAndDropEnabledProperty, new GUIContent("Enabled"));
			if (this.m_DragAndDropEnabledProperty.boolValue)
			{
				EditorGUILayout.PropertyField(this.m_DragKeyModifierProperty, new GUIContent("Drag Key Modifier"));
				EditorGUILayout.PropertyField(this.m_IsStaticProperty, new GUIContent("Is Static"));
				EditorGUILayout.PropertyField(this.m_AllowThrowAwayProperty, new GUIContent("Allow Throw Away"));
			}
			
			EditorGUIUtility.labelWidth = 120f;
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected void DrawTooltipProperties()
		{
			EditorGUILayout.LabelField("Tooltip Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUIUtility.labelWidth = 150f;
			
			EditorGUILayout.PropertyField(this.m_TooltipEnabledProperty, new GUIContent("Enabled"));
			if (this.m_TooltipEnabledProperty.boolValue)
			{
				EditorGUILayout.PropertyField(this.m_TooltipDelayProperty, new GUIContent("Display Delay"));
			}
			
			EditorGUIUtility.labelWidth = 120f;
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected void DrawHoverProperties()
		{
			EditorGUILayout.LabelField("Hovered State Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.hoverTargetGraphicProperty, new GUIContent("Target Graphic"));
			EditorGUILayout.PropertyField(this.hoverTransitionProperty, new GUIContent("Transition"));
			
			Graphic graphic = this.hoverTargetGraphicProperty.objectReferenceValue as Graphic;
			UISlotBase.Transition transition = (UISlotBase.Transition)this.hoverTransitionProperty.enumValueIndex;
			
			if (transition != UISlotBase.Transition.None)
			{
				EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
				
				if (transition == UISlotBase.Transition.ColorTint)
				{
					if (graphic == null)
					{
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Info);
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.hoverNormalColorProperty, new GUIContent("Normal"));
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(this.hoverNormalColorProperty.colorValue);
							
						EditorGUILayout.PropertyField(this.hoverHighlightColorProperty, new GUIContent("Highlighted"));
						EditorGUILayout.PropertyField(this.hoverTransitionDurationProperty, new GUIContent("Duration"));
					}
				}
				else if (transition == UISlotBase.Transition.SpriteSwap)
				{
					if (graphic as Image == null)
					{
						EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					}
					else
					{
						EditorGUILayout.PropertyField(this.hoverOverrideSpriteProperty, new GUIContent("Override Sprite"));
					}
				}
				else if (transition == UISlotBase.Transition.Animation)
				{
					if (graphic == null)
					{
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a animation transition.", MessageType.Info);
					}
					else
					{
						EditorGUILayout.PropertyField(this.hoverNormalTriggerProperty, new GUIContent("Normal"));
						EditorGUILayout.PropertyField(this.hoverHighlightTriggerProperty, new GUIContent("Highlighted"));
						
						Animator animator = graphic.gameObject.GetComponent<Animator>();
						
						if (animator == null || animator.runtimeAnimatorController == null)
						{
							Rect controlRect = EditorGUILayout.GetControlRect();
							controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);
							
							if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
							{
								// Generate the animator controller
								UnityEditor.Animations.AnimatorController animatorController = this.GenerateHoverAnimatorController();
								
								if (animatorController != null)
								{
									if (animator == null)
									{
										animator = graphic.gameObject.AddComponent<Animator>();
									}
									UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
								}
							}
						}
					}
				}
				
				EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected void DrawPressProperties()
		{
			EditorGUILayout.LabelField("Pressed State Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.pressTargetGraphicProperty, new GUIContent("Target Graphic"));
			EditorGUILayout.PropertyField(this.pressTransitionProperty, new GUIContent("Transition"));
			
			Graphic graphic = this.pressTargetGraphicProperty.objectReferenceValue as Graphic;
			UISlotBase.Transition transition = (UISlotBase.Transition)this.pressTransitionProperty.enumValueIndex;
			
			if (transition != UISlotBase.Transition.None)
			{
				EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
				
				if (transition == UISlotBase.Transition.ColorTint)
				{
					if (graphic == null)
					{
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Info);
					}
					else
					{
						EditorGUI.BeginChangeCheck();
						EditorGUILayout.PropertyField(this.pressNormalColorProperty, new GUIContent("Normal"));
						if (EditorGUI.EndChangeCheck())
							graphic.canvasRenderer.SetColor(this.pressNormalColorProperty.colorValue);
							
						EditorGUILayout.PropertyField(this.pressPressColorProperty, new GUIContent("Pressed"));
						EditorGUILayout.PropertyField(this.pressTransitionDurationProperty, new GUIContent("Duration"));
						EditorGUIUtility.labelWidth = 150f;
						EditorGUILayout.PropertyField(this.pressTransitionInstaOutProperty, new GUIContent("Instant Out"));
						EditorGUIUtility.labelWidth = 120f;
					}
				}
				else if (transition == UISlotBase.Transition.SpriteSwap)
				{
					if (graphic as Image == null)
					{
						EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
					}
					else
					{
						EditorGUILayout.PropertyField(this.pressOverrideSpriteProperty, new GUIContent("Override Sprite"));
					}
				}
				else if (transition == UISlotBase.Transition.Animation)
				{
					if (graphic == null)
					{
						EditorGUILayout.HelpBox("You must have a Graphic target in order to use a animation transition.", MessageType.Info);
					}
					else
					{
						EditorGUILayout.PropertyField(this.pressNormalTriggerProperty, new GUIContent("Normal"));
						EditorGUILayout.PropertyField(this.pressPressTriggerProperty, new GUIContent("Pressed"));
						
						Animator animator = graphic.gameObject.GetComponent<Animator>();
						
						if (animator == null || animator.runtimeAnimatorController == null)
						{
							Rect controlRect = EditorGUILayout.GetControlRect();
							controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);
							
							if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
							{
								// Generate the animator controller
								UnityEditor.Animations.AnimatorController animatorController = this.GeneratePressAnimatorController();
								
								if (animatorController != null)
								{
									if (animator == null)
									{
										animator = graphic.gameObject.AddComponent<Animator>();
									}
									UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
								}
							}
						}
					}
				}
				
				EditorGUIUtility.labelWidth = 150f;
				EditorGUILayout.PropertyField(this.pressForceHoverNormalProperty, new GUIContent("Force Hover Normal"));
				EditorGUIUtility.labelWidth = 120f;
				EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
		
		protected UnityEditor.Animations.AnimatorController GenerateHoverAnimatorController()
		{
			// Prepare the triggers list
			List<string> triggers = new List<string>();
			
			triggers.Add((!string.IsNullOrEmpty(this.hoverNormalTriggerProperty.stringValue)) ? this.hoverNormalTriggerProperty.stringValue : "Normal");
			triggers.Add((!string.IsNullOrEmpty(this.hoverHighlightTriggerProperty.stringValue)) ? this.hoverHighlightTriggerProperty.stringValue : "Highlighted");
			
			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers, this.hoverTargetGraphicProperty.objectReferenceValue.name);
		}
		
		protected UnityEditor.Animations.AnimatorController GeneratePressAnimatorController()
		{
			// Prepare the triggers list
			List<string> triggers = new List<string>();
			
			triggers.Add((!string.IsNullOrEmpty(this.pressNormalTriggerProperty.stringValue)) ? this.pressNormalTriggerProperty.stringValue : "Normal");
			triggers.Add((!string.IsNullOrEmpty(this.pressPressTriggerProperty.stringValue)) ? this.pressPressTriggerProperty.stringValue : "Pressed");
			
			return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers, this.pressTargetGraphicProperty.objectReferenceValue.name);
		}
	}
}
