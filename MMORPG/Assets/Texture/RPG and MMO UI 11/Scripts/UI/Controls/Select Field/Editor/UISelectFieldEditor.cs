using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UI;
using System;
using System.Collections.Generic;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CustomEditor(typeof(UISelectField), true)]
	public class UISelectFieldEditor : SelectableEditor {

        public const string PREFS_KEY = "UISelectFieldEditor_";
        private bool showSelectLayout = true;
        private bool showListLayout = true;
        private bool showScrollRectLayout = true;
        private bool showListSeparatorLayout = true;
        private bool showOptionLayout = true;
        private bool showOptionBackgroundLayout = true;
        private bool showOptionHover = true;
        private bool showOptionPress = true;
        private GUIStyle m_FoldoutStyle;

        private SerializedProperty m_DirectionProperty;
		private SerializedProperty m_TargetGraphicProperty;
		private SerializedProperty m_InteractableProperty;
		private SerializedProperty m_TransitionProperty;
		private SerializedProperty m_ColorBlockProperty;
		private SerializedProperty m_SpriteStateProperty;
		private SerializedProperty m_AnimTriggerProperty;
		private SerializedProperty m_NavigationProperty;
		private SerializedProperty m_LabelTextProperty;
		private SerializedProperty m_ListBackgroundSpriteProperty;
		private SerializedProperty m_ListBackgroundSpriteTypeProperty;
		private SerializedProperty m_ListBackgroundColorProperty;
		private SerializedProperty m_ListMarginsProperty;
		private SerializedProperty m_ListPaddingProperty;
		private SerializedProperty m_ListSpacingProperty;
        private SerializedProperty m_ListSeparatorSpriteProperty;
		private SerializedProperty m_ListSeparatorTypeProperty;
		private SerializedProperty m_ListSeparatorColorProperty;
		private SerializedProperty m_ListSeparatorHeightProperty;
        private SerializedProperty m_ListStartSeparator;
		private SerializedProperty m_ListAnimationTypeProperty;
		private SerializedProperty m_ListAnimationDurationProperty;
		private SerializedProperty m_ListAnimatorControllerProperty;
		private SerializedProperty m_ListlistAnimationOpenTriggerProperty;
		private SerializedProperty m_ListlistAnimationCloseTriggerProperty;
		private SerializedProperty m_OptionFontProperty;
		private SerializedProperty m_OptionFontSizeProperty;
		private SerializedProperty m_OptionFontStyleProperty;
		private SerializedProperty m_OptionColorProperty;
		private SerializedProperty m_OptionTextTransitionColorsProperty;
		private SerializedProperty m_OptionPaddingProperty;
		private SerializedProperty m_OptionTextEffectTypeProperty;
		private SerializedProperty m_OptionTextEffectColorProperty;
		private SerializedProperty m_OptionTextEffectDistanceProperty;
		private SerializedProperty m_OptionTextEffectUseGraphicAlphaProperty;
		private SerializedProperty m_OptionTextTransitionTypeProperty;
		private SerializedProperty m_OptionBackgroundSpriteProperty;
		private SerializedProperty m_OptionBackgroundSpriteTypeProperty;
		private SerializedProperty m_OptionBackgroundSpriteColorProperty;
		private SerializedProperty m_OptionBackgroundTransitionTypeProperty;
		private SerializedProperty m_OptionBackgroundTransColorsProperty;
		private SerializedProperty m_OptionBackgroundSpriteStatesProperty;
		private SerializedProperty m_OptionBackgroundAnimationTriggersProperty;
		private SerializedProperty m_OptionBackgroundAnimatorControllerProperty;
        private SerializedProperty m_OptionHoverOverlayProperty;
        private SerializedProperty m_OptionHoverOverlayColorProperty;
        private SerializedProperty m_OptionHoverOverlayColorBlockProperty;
        private SerializedProperty m_OptionPressOverlayProperty;
        private SerializedProperty m_OptionPressOverlayColorProperty;
        private SerializedProperty m_OptionPressOverlayColorBlockProperty;
        private SerializedProperty m_OnChangeProperty;
        private SerializedProperty m_AllowScrollRectProperty;
        private SerializedProperty m_ScrollMovementTypeProperty;
        private SerializedProperty m_ScrollElasticityProperty;
        private SerializedProperty m_ScrollInertiaProperty;
        private SerializedProperty m_ScrollDecelerationRateProperty;
        private SerializedProperty m_ScrollSensitivityProperty;
        private SerializedProperty m_ScrollMinOptionsProperty;
        private SerializedProperty m_ScrollListHeightProperty;
        private SerializedProperty m_ScrollBarPrefabProperty;
        private SerializedProperty m_ScrollbarSpacingProperty;

        protected override void OnEnable()
		{
			base.OnEnable();

            this.showSelectLayout = EditorPrefs.GetBool(PREFS_KEY + "1", true);
            this.showListLayout = EditorPrefs.GetBool(PREFS_KEY + "2", true);
            this.showListSeparatorLayout = EditorPrefs.GetBool(PREFS_KEY + "3", true);
            this.showOptionLayout = EditorPrefs.GetBool(PREFS_KEY + "4", true);
            this.showOptionBackgroundLayout = EditorPrefs.GetBool(PREFS_KEY + "5", true);
            this.showOptionHover = EditorPrefs.GetBool(PREFS_KEY + "6", true);
            this.showOptionPress = EditorPrefs.GetBool(PREFS_KEY + "7", true);
            this.showScrollRectLayout = EditorPrefs.GetBool(PREFS_KEY + "8", true);

            this.m_TargetGraphicProperty = base.serializedObject.FindProperty("m_TargetGraphic");
			this.m_InteractableProperty = base.serializedObject.FindProperty("m_Interactable");
			this.m_TransitionProperty = base.serializedObject.FindProperty("m_Transition");
			this.m_NavigationProperty = base.serializedObject.FindProperty("m_Navigation");
			this.m_ColorBlockProperty = this.serializedObject.FindProperty("colors");
			this.m_SpriteStateProperty = this.serializedObject.FindProperty("spriteState");
			this.m_AnimTriggerProperty = this.serializedObject.FindProperty("animationTriggers");
			this.m_DirectionProperty = this.serializedObject.FindProperty("m_Direction");
			this.m_LabelTextProperty = this.serializedObject.FindProperty("m_LabelText");
			this.m_ListBackgroundSpriteProperty = this.serializedObject.FindProperty("listBackgroundSprite");
			this.m_ListBackgroundSpriteTypeProperty = this.serializedObject.FindProperty("listBackgroundSpriteType");
			this.m_ListBackgroundColorProperty = this.serializedObject.FindProperty("listBackgroundColor");
			this.m_ListMarginsProperty = this.serializedObject.FindProperty("listMargins");
			this.m_ListPaddingProperty = this.serializedObject.FindProperty("listPadding");
			this.m_ListSpacingProperty = this.serializedObject.FindProperty("listSpacing");
            this.m_ListSeparatorSpriteProperty = this.serializedObject.FindProperty("listSeparatorSprite");
			this.m_ListSeparatorTypeProperty = this.serializedObject.FindProperty("listSeparatorType");
			this.m_ListSeparatorColorProperty = this.serializedObject.FindProperty("listSeparatorColor");
			this.m_ListSeparatorHeightProperty = this.serializedObject.FindProperty("listSeparatorHeight");
            this.m_ListStartSeparator = this.serializedObject.FindProperty("startSeparator");
            this.m_ListAnimationTypeProperty = this.serializedObject.FindProperty("listAnimationType");
			this.m_ListAnimationDurationProperty = this.serializedObject.FindProperty("listAnimationDuration");
			this.m_ListAnimatorControllerProperty = this.serializedObject.FindProperty("listAnimatorController");
			this.m_ListlistAnimationOpenTriggerProperty = this.serializedObject.FindProperty("listAnimationOpenTrigger");
			this.m_ListlistAnimationCloseTriggerProperty = this.serializedObject.FindProperty("listAnimationCloseTrigger");
			this.m_OptionFontProperty = this.serializedObject.FindProperty("optionFont");
			this.m_OptionFontSizeProperty = this.serializedObject.FindProperty("optionFontSize");
			this.m_OptionFontStyleProperty = this.serializedObject.FindProperty("optionFontStyle");
			this.m_OptionColorProperty = this.serializedObject.FindProperty("optionColor");
			this.m_OptionTextTransitionColorsProperty = this.serializedObject.FindProperty("optionTextTransitionColors");
			this.m_OptionPaddingProperty = this.serializedObject.FindProperty("optionPadding");
			this.m_OptionTextEffectTypeProperty = this.serializedObject.FindProperty("optionTextEffectType");
			this.m_OptionTextEffectColorProperty = this.serializedObject.FindProperty("optionTextEffectColor");
			this.m_OptionTextEffectDistanceProperty = this.serializedObject.FindProperty("optionTextEffectDistance");
			this.m_OptionTextEffectUseGraphicAlphaProperty = this.serializedObject.FindProperty("optionTextEffectUseGraphicAlpha");
			this.m_OptionTextTransitionTypeProperty = this.serializedObject.FindProperty("optionTextTransitionType");
			this.m_OptionBackgroundSpriteProperty = this.serializedObject.FindProperty("optionBackgroundSprite");
			this.m_OptionBackgroundSpriteTypeProperty = this.serializedObject.FindProperty("optionBackgroundSpriteType");
			this.m_OptionBackgroundSpriteColorProperty = this.serializedObject.FindProperty("optionBackgroundSpriteColor");
			this.m_OptionBackgroundTransitionTypeProperty = this.serializedObject.FindProperty("optionBackgroundTransitionType");
			this.m_OptionBackgroundTransColorsProperty = this.serializedObject.FindProperty("optionBackgroundTransColors");
			this.m_OptionBackgroundSpriteStatesProperty = this.serializedObject.FindProperty("optionBackgroundSpriteStates");
			this.m_OptionBackgroundAnimationTriggersProperty = this.serializedObject.FindProperty("optionBackgroundAnimationTriggers");
			this.m_OptionBackgroundAnimatorControllerProperty = this.serializedObject.FindProperty("optionBackgroundAnimatorController");
            this.m_OptionHoverOverlayProperty = this.serializedObject.FindProperty("optionHoverOverlay");
            this.m_OptionHoverOverlayColorProperty = this.serializedObject.FindProperty("optionHoverOverlayColor");
            this.m_OptionHoverOverlayColorBlockProperty = this.serializedObject.FindProperty("optionHoverOverlayColorBlock");
            this.m_OptionPressOverlayProperty = this.serializedObject.FindProperty("optionPressOverlay");
            this.m_OptionPressOverlayColorProperty = this.serializedObject.FindProperty("optionPressOverlayColor");
            this.m_OptionPressOverlayColorBlockProperty = this.serializedObject.FindProperty("optionPressOverlayColorBlock");

            this.m_AllowScrollRectProperty = this.serializedObject.FindProperty("allowScrollRect");
            this.m_ScrollMovementTypeProperty = this.serializedObject.FindProperty("scrollMovementType");
            this.m_ScrollElasticityProperty = this.serializedObject.FindProperty("scrollElasticity");
            this.m_ScrollInertiaProperty = this.serializedObject.FindProperty("scrollInertia");
            this.m_ScrollDecelerationRateProperty = this.serializedObject.FindProperty("scrollDecelerationRate");
            this.m_ScrollSensitivityProperty = this.serializedObject.FindProperty("scrollSensitivity");
            this.m_ScrollMinOptionsProperty = this.serializedObject.FindProperty("scrollMinOptions");
            this.m_ScrollListHeightProperty = this.serializedObject.FindProperty("scrollListHeight");
            this.m_ScrollBarPrefabProperty = this.serializedObject.FindProperty("scrollBarPrefab");
            this.m_ScrollbarSpacingProperty = this.serializedObject.FindProperty("scrollbarSpacing");

            this.m_OnChangeProperty = this.serializedObject.FindProperty("onChange");
		}
		
		public override void OnInspectorGUI()
		{
            if (this.m_FoldoutStyle == null)
            {
                this.m_FoldoutStyle = new GUIStyle(EditorStyles.foldout);
                this.m_FoldoutStyle.normal.textColor = Color.black;
                this.m_FoldoutStyle.fontStyle = FontStyle.Bold;
            }

            UISelectField select = (this.target as UISelectField);
			this.serializedObject.Update();

			this.DrawOptionsArea();
			EditorGUILayout.Separator();
			UISelectFieldEditor.DrawStringPopup("Default option", select.options.ToArray(), select.value, OnDefaultOptionSelected);
			EditorGUILayout.PropertyField(this.m_DirectionProperty);
			EditorGUILayout.PropertyField(this.m_InteractableProperty, new GUIContent("Interactable"));
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.m_LabelTextProperty, new GUIContent("Label Text"));
			EditorGUILayout.Separator();
			this.DrawSelectFieldLayotProperties();
			EditorGUILayout.Separator();
			this.DrawListLayoutProperties();
			EditorGUILayout.Separator();
            this.DrawScrollRectProperties();
            EditorGUILayout.Separator();
			this.DrawListSeparatorLayoutProperties();
			EditorGUILayout.Separator();
			this.DrawOptionLayoutProperties();
			EditorGUILayout.Separator();
			this.DrawOptionBackgroundLayoutProperties();
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.m_NavigationProperty);
			EditorGUILayout.Separator();
			EditorGUILayout.PropertyField(this.m_OnChangeProperty);
			
			this.serializedObject.ApplyModifiedProperties();
		}

		/// <summary>
		/// Raises the default option selected event.
		/// </summary>
		/// <param name="value">Value.</param>
		private void OnDefaultOptionSelected(string value)
		{
			UISelectField select = (this.target as UISelectField);
			
			Undo.RecordObject(select, "Select Field default option changed.");
			select.SelectOption(value);
			EditorUtility.SetDirty(select);
		}
		
		/// <summary>
		/// Draws the options area.
		/// </summary>
		private void DrawOptionsArea()
		{
			UISelectField select = (this.target as UISelectField);
			
			// Place a label for the options
			EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
			
			// Prepare the string to be used in the text area
			string text = "";
			foreach (string s in select.options)
				text += s + "\n";
			
			string modified = EditorGUILayout.TextArea(text, GUI.skin.textArea, GUILayout.Height(100f));
			
			// Check if the options have changed
			if (!modified.Equals(text))
			{
				Undo.RecordObject(target, "UI Select Field changed.");
				
				string[] split = modified.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
				
				select.ClearOptions();
				
				foreach (string s in split)
					select.AddOption(s);
				
				if (string.IsNullOrEmpty(select.value) || !select.options.Contains(select.value))
				{
					select.value = select.options.Count > 0 ? select.options[0] : "";
				}
				
				EditorUtility.SetDirty(target);
			}
		}
		
		/// <summary>
		/// Draws the select field layot properties.
		/// </summary>
		public void DrawSelectFieldLayotProperties()
		{
            bool newState = EditorGUILayout.Foldout(this.showSelectLayout, "Select Field Layout", this.m_FoldoutStyle);

            if (newState != this.showSelectLayout)
            {
                EditorPrefs.SetBool(PREFS_KEY + "1", newState);
                this.showSelectLayout = newState;
            }

            if (this.showSelectLayout)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

                EditorGUILayout.PropertyField(this.m_TransitionProperty, new GUIContent("Transition"));

                Graphic graphic = this.m_TargetGraphicProperty.objectReferenceValue as Graphic;
                Selectable.Transition transition = (Selectable.Transition)this.m_TransitionProperty.enumValueIndex;

                // Check if the transition requires a graphic
                if (transition == Selectable.Transition.ColorTint || transition == Selectable.Transition.SpriteSwap)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_TargetGraphicProperty);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }

                // Check if we have a transition set
                if (transition != Selectable.Transition.None)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

                    if (transition == Selectable.Transition.ColorTint)
                    {
                        if (graphic == null)
                        {
                            EditorGUILayout.HelpBox("You must have a Graphic target in order to use a color transition.", MessageType.Info);
                        }
                        else
                        {
                            EditorGUI.BeginChangeCheck();
                            EditorGUILayout.PropertyField(this.m_ColorBlockProperty, true);
                            if (EditorGUI.EndChangeCheck())
                                graphic.canvasRenderer.SetColor(this.m_ColorBlockProperty.FindPropertyRelative("m_NormalColor").colorValue);
                        }
                    }
                    else if (transition == Selectable.Transition.SpriteSwap)
                    {
                        if (graphic as Image == null)
                        {
                            EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
                        }
                        else
                        {
                            EditorGUILayout.PropertyField(this.m_SpriteStateProperty, true);
                        }
                    }
                    else if (transition == Selectable.Transition.Animation)
                    {
                        EditorGUILayout.PropertyField(this.m_AnimTriggerProperty, true);

                        Animator animator = (target as UISelectField).animator;

                        if (animator == null || animator.runtimeAnimatorController == null)
                        {
                            Rect controlRect = EditorGUILayout.GetControlRect();
                            controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);

                            if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
                            {
                                // Generate the animator controller
                                UnityEditor.Animations.AnimatorController animatorController = UIAnimatorControllerGenerator.GenerateAnimatorContoller(this.m_AnimTriggerProperty, target.name);

                                if (animatorController != null)
                                {
                                    if (animator == null)
                                    {
                                        animator = (target as UISelectField).gameObject.AddComponent<Animator>();
                                    }
                                    UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
                                }
                            }
                        }
                    }

                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }

                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
		} 
		
		/// <summary>
		/// Draws the list layout properties.
		/// </summary>
		public void DrawListLayoutProperties()
		{
            bool newState = EditorGUILayout.Foldout(this.showListLayout, "List Layout", this.m_FoldoutStyle);

            if (newState != this.showListLayout)
            {
                EditorPrefs.SetBool(PREFS_KEY + "2", newState);
                this.showListLayout = newState;
            }

            if (this.showListLayout)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_ListBackgroundSpriteProperty, new GUIContent("Sprite"));
                if (this.m_ListBackgroundSpriteProperty.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(this.m_ListBackgroundSpriteTypeProperty, new GUIContent("Sprite Type"));
                    EditorGUILayout.PropertyField(this.m_ListBackgroundColorProperty, new GUIContent("Sprite Color"));
                }
                EditorGUILayout.PropertyField(this.m_ListMarginsProperty, new GUIContent("Margin"), true);
                EditorGUILayout.PropertyField(this.m_ListPaddingProperty, new GUIContent("Padding"), true);
                EditorGUILayout.PropertyField(this.m_ListSpacingProperty, new GUIContent("Spacing"), true);
                EditorGUILayout.PropertyField(this.m_ListAnimationTypeProperty, new GUIContent("Transition"), true);

                UISelectField.ListAnimationType animationType = (UISelectField.ListAnimationType)this.m_ListAnimationTypeProperty.enumValueIndex;

                if (animationType == UISelectField.ListAnimationType.Fade)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_ListAnimationDurationProperty, new GUIContent("Duration"), true);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }
                else if (animationType == UISelectField.ListAnimationType.Animation)
                {
                    EditorGUILayout.PropertyField(this.m_ListAnimatorControllerProperty, new GUIContent("Animator Controller"));
                    EditorGUILayout.PropertyField(this.m_ListlistAnimationOpenTriggerProperty, new GUIContent("Open Trigger"));
                    EditorGUILayout.PropertyField(this.m_ListlistAnimationCloseTriggerProperty, new GUIContent("Close Trigger"));

                    if (this.m_ListAnimatorControllerProperty.objectReferenceValue == null)
                    {
                        Rect controlRect = EditorGUILayout.GetControlRect();
                        controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);

                        if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
                        {
                            // Prepare the triggers list
                            List<string> triggers = new List<string>();
                            triggers.Add(!string.IsNullOrEmpty(this.m_ListlistAnimationOpenTriggerProperty.stringValue) ? this.m_ListlistAnimationOpenTriggerProperty.stringValue : "Open");
                            triggers.Add(!string.IsNullOrEmpty(this.m_ListlistAnimationCloseTriggerProperty.stringValue) ? this.m_ListlistAnimationCloseTriggerProperty.stringValue : "Close");

                            // Generate the animator controller
                            UnityEditor.Animations.AnimatorController animatorController = UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers, target.name + " - List");

                            // Apply the controller to the property
                            if (animatorController != null)
                                this.m_ListAnimatorControllerProperty.objectReferenceValue = animatorController;
                        }
                    }
                }
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
		}

        /// <summary>
        /// Draws the scroll rect properties.
        /// </summary>
        public void DrawScrollRectProperties()
        {
            bool newState = EditorGUILayout.Foldout(this.showScrollRectLayout, "Scrolling Properties", this.m_FoldoutStyle);

            if (newState != this.showScrollRectLayout)
            {
                EditorPrefs.SetBool(PREFS_KEY + "8", newState);
                this.showScrollRectLayout = newState;
            }

            if (this.showScrollRectLayout)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

                EditorGUILayout.PropertyField(this.m_AllowScrollRectProperty, new GUIContent("Allow Scroll Rect"));
                EditorGUILayout.PropertyField(this.m_ScrollMovementTypeProperty, new GUIContent("Movement Type"));

                if (this.m_ScrollMovementTypeProperty.enumValueIndex == 1)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_ScrollElasticityProperty, new GUIContent("Elasticity"));
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }

                EditorGUILayout.PropertyField(this.m_ScrollInertiaProperty, new GUIContent("Inertia"));

                if (this.m_ScrollInertiaProperty.boolValue)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_ScrollDecelerationRateProperty, new GUIContent("Deceleration Rate"));
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }

                EditorGUILayout.PropertyField(this.m_ScrollSensitivityProperty, new GUIContent("Scroll Sensitivity"));
                EditorGUILayout.PropertyField(this.m_ScrollMinOptionsProperty, new GUIContent("Minimum Options", "The number of options required to use the scroll rect."));
                EditorGUILayout.PropertyField(this.m_ScrollListHeightProperty, new GUIContent("List Height"));
                EditorGUILayout.PropertyField(this.m_ScrollBarPrefabProperty, new GUIContent("Scroll Bar Prefab"));
                EditorGUILayout.PropertyField(this.m_ScrollbarSpacingProperty, new GUIContent("Scroll Bar Spacing"));

                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
        }

		/// <summary>
		/// Draws the list separator layout properties.
		/// </summary>
		public void DrawListSeparatorLayoutProperties()
		{
            bool newState = EditorGUILayout.Foldout(this.showListSeparatorLayout, "List Separator Layout", this.m_FoldoutStyle);

            if (newState != this.showListSeparatorLayout)
            {
                EditorPrefs.SetBool(PREFS_KEY + "3", newState);
                this.showListSeparatorLayout = newState;
            }

            if (this.showListSeparatorLayout)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

                EditorGUILayout.PropertyField(this.m_ListSeparatorSpriteProperty, new GUIContent("Sprite"));

                if (this.m_ListSeparatorSpriteProperty.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(this.m_ListSeparatorTypeProperty, new GUIContent("Sprite Type"));
                    EditorGUILayout.PropertyField(this.m_ListSeparatorColorProperty, new GUIContent("Sprite Color"));
                    EditorGUILayout.PropertyField(this.m_ListSeparatorHeightProperty, new GUIContent("Override Height"));
                    EditorGUILayout.PropertyField(this.m_ListStartSeparator, new GUIContent("Start Separator"));
                }

                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
		}

		/// <summary>
		/// Draws the option layout properties.
		/// </summary>
		public void DrawOptionLayoutProperties()
		{
            bool newState = EditorGUILayout.Foldout(this.showOptionLayout, "Option Layout", this.m_FoldoutStyle);

            if (newState != this.showOptionLayout)
            {
                EditorPrefs.SetBool(PREFS_KEY + "4", newState);
                this.showOptionLayout = newState;
            }

            if (this.showOptionLayout)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_OptionFontProperty, new GUIContent("Font"));
                EditorGUILayout.PropertyField(this.m_OptionFontSizeProperty, new GUIContent("Font size"));
                EditorGUILayout.PropertyField(this.m_OptionFontStyleProperty, new GUIContent("Font style"));
                EditorGUILayout.PropertyField(this.m_OptionColorProperty, new GUIContent("Color Normal"));
                EditorGUILayout.PropertyField(this.m_OptionPaddingProperty, new GUIContent("Padding"), true);
                EditorGUILayout.PropertyField(this.m_OptionTextEffectTypeProperty, new GUIContent("Effect Type"));

                UISelectField.OptionTextEffectType textEffect = (UISelectField.OptionTextEffectType)this.m_OptionTextEffectTypeProperty.enumValueIndex;

                if (textEffect != UISelectField.OptionTextEffectType.None)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_OptionTextEffectColorProperty, new GUIContent("Color"), true);
                    EditorGUILayout.PropertyField(this.m_OptionTextEffectDistanceProperty, new GUIContent("Distance"), true);
                    EditorGUILayout.PropertyField(this.m_OptionTextEffectUseGraphicAlphaProperty, new GUIContent("Use graphic alpha"), true);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }

                EditorGUILayout.PropertyField(this.m_OptionTextTransitionTypeProperty, new GUIContent("Transition"));

                UISelectField.OptionTextTransitionType textTransition = (UISelectField.OptionTextTransitionType)this.m_OptionTextTransitionTypeProperty.enumValueIndex;

                if (textTransition == UISelectField.OptionTextTransitionType.CrossFade)
                {
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_OptionTextTransitionColorsProperty, true);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
		}
		
		/// <summary>
		/// Draws the option background layout properties.
		/// </summary>
		public void DrawOptionBackgroundLayoutProperties()
		{
            bool newState = EditorGUILayout.Foldout(this.showOptionBackgroundLayout, "Option Background Layout", this.m_FoldoutStyle);

            if (newState != this.showOptionBackgroundLayout)
            {
                EditorPrefs.SetBool(PREFS_KEY + "5", newState);
                this.showOptionBackgroundLayout = newState;
            }

            if (this.showOptionBackgroundLayout)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_OptionBackgroundSpriteProperty, new GUIContent("Sprite"));

                if (this.m_OptionBackgroundSpriteProperty.objectReferenceValue != null)
                {
                    EditorGUILayout.PropertyField(this.m_OptionBackgroundSpriteTypeProperty, new GUIContent("Sprite Type"));
                    EditorGUILayout.PropertyField(this.m_OptionBackgroundSpriteColorProperty, new GUIContent("Sprite Color"));
                    EditorGUILayout.PropertyField(this.m_OptionBackgroundTransitionTypeProperty, new GUIContent("Transition"));

                    Selectable.Transition optionBgTransition = (Selectable.Transition)this.m_OptionBackgroundTransitionTypeProperty.enumValueIndex;

                    if (optionBgTransition != Selectable.Transition.None)
                    {
                        EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                        if (optionBgTransition == Selectable.Transition.ColorTint)
                        {
                            EditorGUILayout.PropertyField(this.m_OptionBackgroundTransColorsProperty, true);
                        }
                        else if (optionBgTransition == Selectable.Transition.SpriteSwap)
                        {
                            EditorGUILayout.PropertyField(this.m_OptionBackgroundSpriteStatesProperty, true);
                        }
                        else if (optionBgTransition == Selectable.Transition.Animation)
                        {
                            EditorGUILayout.PropertyField(this.m_OptionBackgroundAnimatorControllerProperty, new GUIContent("Animator Controller"));
                            EditorGUILayout.PropertyField(this.m_OptionBackgroundAnimationTriggersProperty, true);

                            if (this.m_OptionBackgroundAnimatorControllerProperty.objectReferenceValue == null)
                            {
                                Rect controlRect = EditorGUILayout.GetControlRect();
                                controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);

                                if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
                                {
                                    // Generate the animator controller
                                    UnityEditor.Animations.AnimatorController animatorController = UIAnimatorControllerGenerator.GenerateAnimatorContoller(this.m_OptionBackgroundAnimationTriggersProperty, target.name + " - Option Background");

                                    // Apply the controller to the property
                                    if (animatorController != null)
                                        this.m_OptionBackgroundAnimatorControllerProperty.objectReferenceValue = animatorController;
                                }
                            }
                        }
                        EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                    }
                }
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }

            EditorGUILayout.Separator();

            bool newState2 = EditorGUILayout.Foldout(this.showOptionHover, "Option Hover Overlay", this.m_FoldoutStyle);

            if (newState2 != this.showOptionHover)
            {
                EditorPrefs.SetBool(PREFS_KEY + "6", newState2);
                this.showOptionHover = newState2;
            }

            if (this.showOptionHover)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_OptionHoverOverlayProperty, new GUIContent("Sprite"));
                EditorGUILayout.PropertyField(this.m_OptionHoverOverlayColorProperty, new GUIContent("Sprite Color"));
                if (this.m_OptionHoverOverlayProperty.objectReferenceValue != null)
                {
                    EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_OptionHoverOverlayColorBlockProperty, new GUIContent("Colors"), true);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }

            EditorGUILayout.Separator();

            bool newState3 = EditorGUILayout.Foldout(this.showOptionPress, "Option Press Overlay", this.m_FoldoutStyle);

            if (newState3 != this.showOptionPress)
            {
                EditorPrefs.SetBool(PREFS_KEY + "7", newState3);
                this.showOptionPress = newState3;
            }

            if (this.showOptionPress)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_OptionPressOverlayProperty, new GUIContent("Sprite"));
                EditorGUILayout.PropertyField(this.m_OptionPressOverlayColorProperty, new GUIContent("Sprite Color"));
                if (this.m_OptionPressOverlayProperty.objectReferenceValue != null)
                {
                    EditorGUILayout.LabelField("Transition", EditorStyles.boldLabel);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                    EditorGUILayout.PropertyField(this.m_OptionPressOverlayColorBlockProperty, new GUIContent("Colors"), true);
                    EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
                }
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
        }
		
		/// <summary>
		/// Draws a string popup field.
		/// </summary>
		/// <param name="label">Label.</param>
		/// <param name="list">Array of values.</param>
		/// <param name="selected">The selected value.</param>
		/// <param name="onChange">On change.</param>
		public static void DrawStringPopup(string label, string[] list, string selected, Action<string> onChange)
		{
			string newValue = string.Empty;
			GUI.changed = false;
			
			if (list != null && list.Length > 0)
			{
				int index = 0;
				
				// Make sure we have a selection
				if (string.IsNullOrEmpty(selected))
					selected = list[0];
				
				// Find the index of the selection
				else if (!string.IsNullOrEmpty(selected))
				{
					for (int i = 0; i < list.Length; ++i)
					{
						if (selected.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
						{
							index = i;
							break;
						}
					}
				}
				
				// Draw the sprite selection popup
				index = string.IsNullOrEmpty(label) ? EditorGUILayout.Popup(index, list) : EditorGUILayout.Popup(label, index, list);
				
				// Save the selected value
				newValue = list[index];
			}
			
			// Invoke the event with the selected value
			if (GUI.changed)
				onChange.Invoke(newValue);
		}
	}
}
