using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections.Generic;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(UIPressTransition))]
    public class UIPressTransitionEditor : Editor
    {
        private SerializedProperty m_TransitionProperty;
        private SerializedProperty m_TargetGraphicProperty;
        private SerializedProperty m_TargetGameObjectProperty;
        private SerializedProperty m_NormalColorProperty;
        private SerializedProperty m_PressedColorProperty;
        private SerializedProperty m_DurationProperty;
        private SerializedProperty m_ColorMultiplierProperty;
        private SerializedProperty m_PressedSpriteProperty;
        private SerializedProperty m_NormalTriggerProperty;
        private SerializedProperty m_PressedTriggerProperty;

        protected void OnEnable()
        {
            this.m_TransitionProperty = this.serializedObject.FindProperty("m_Transition");
            this.m_TargetGraphicProperty = this.serializedObject.FindProperty("m_TargetGraphic");
            this.m_TargetGameObjectProperty = this.serializedObject.FindProperty("m_TargetGameObject");
            this.m_NormalColorProperty = this.serializedObject.FindProperty("m_NormalColor");
            this.m_PressedColorProperty = this.serializedObject.FindProperty("m_PressedColor");
            this.m_DurationProperty = this.serializedObject.FindProperty("m_Duration");
            this.m_ColorMultiplierProperty = this.serializedObject.FindProperty("m_ColorMultiplier");
            this.m_PressedSpriteProperty = this.serializedObject.FindProperty("m_PressedSprite");
            this.m_NormalTriggerProperty = this.serializedObject.FindProperty("m_NormalTrigger");
            this.m_PressedTriggerProperty = this.serializedObject.FindProperty("m_PressedTrigger");
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            UIHighlightTransition.Transition transition = (UIHighlightTransition.Transition)this.m_TransitionProperty.enumValueIndex;
            Graphic graphic = this.m_TargetGraphicProperty.objectReferenceValue as Graphic;
            GameObject targetGameObject = this.m_TargetGameObjectProperty.objectReferenceValue as GameObject;

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

                        EditorGUILayout.PropertyField(this.m_PressedColorProperty, true);
                        EditorGUILayout.PropertyField(this.m_ColorMultiplierProperty, true);
                        EditorGUILayout.PropertyField(this.m_DurationProperty, true);
                    }
                }
                else if (transition == UIHighlightTransition.Transition.TextColor)
                {
                    if ((graphic is Text) == false)
                    {
                        EditorGUILayout.HelpBox("You must have a Text target in order to use a text color transition.", MessageType.Info);
                    }
                    else
                    {
                        EditorGUI.BeginChangeCheck();
                        EditorGUILayout.PropertyField(this.m_NormalColorProperty, true);
                        if (EditorGUI.EndChangeCheck())
                            (graphic as Text).color = this.m_NormalColorProperty.colorValue;

                        EditorGUILayout.PropertyField(this.m_PressedColorProperty, true);
                        EditorGUILayout.PropertyField(this.m_DurationProperty, true);
                    }
                }
                else if (transition == UIHighlightTransition.Transition.SpriteSwap)
                {
                    if (graphic as Image == null)
                    {
                        EditorGUILayout.HelpBox("You must have a Image target in order to use a sprite swap transition.", MessageType.Info);
                    }
                    else
                    {
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

            this.serializedObject.ApplyModifiedProperties();
        }

        private UnityEditor.Animations.AnimatorController GenerateAnimatorController()
        {
            // Prepare the triggers list
            List<string> triggers = new List<string>();

            triggers.Add((!string.IsNullOrEmpty(this.m_NormalTriggerProperty.stringValue)) ? this.m_NormalTriggerProperty.stringValue : "Normal");
            triggers.Add((!string.IsNullOrEmpty(this.m_PressedTriggerProperty.stringValue)) ? this.m_PressedTriggerProperty.stringValue : "Highlighted");

            return UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggers, this.m_TargetGameObjectProperty.objectReferenceValue.name);
        }
    }
}
