using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CanEditMultipleObjects, CustomEditor(typeof(UIScene))]
    public class UISceneEditor : Editor
    {
        private SerializedProperty m_Id;
        private SerializedProperty m_IsActivated;
        private SerializedProperty m_Type;
        private SerializedProperty m_Content;
        private SerializedProperty m_Prefab;
        private SerializedProperty m_Resource;
        private SerializedProperty m_Transition;
        private SerializedProperty m_TransitionDuration;
        private SerializedProperty m_TransitionEasing;
        private SerializedProperty m_AnimateInTrigger;
        private SerializedProperty m_AnimateOutTrigger;
        private SerializedProperty m_OnActivate;
        private SerializedProperty m_OnDeactivate;
        private SerializedProperty m_FirstSelected;

        protected virtual void OnEnable()
        {
            this.m_Id = this.serializedObject.FindProperty("m_Id");
            this.m_IsActivated = this.serializedObject.FindProperty("m_IsActivated");
            this.m_Type = this.serializedObject.FindProperty("m_Type");
            this.m_Content = this.serializedObject.FindProperty("m_Content");
            this.m_Prefab = this.serializedObject.FindProperty("m_Prefab");
            this.m_Resource = this.serializedObject.FindProperty("m_Resource");
            this.m_Transition = this.serializedObject.FindProperty("m_Transition");
            this.m_TransitionDuration = this.serializedObject.FindProperty("m_TransitionDuration");
            this.m_TransitionEasing = this.serializedObject.FindProperty("m_TransitionEasing");
            this.m_AnimateInTrigger = this.serializedObject.FindProperty("m_AnimateInTrigger");
            this.m_AnimateOutTrigger = this.serializedObject.FindProperty("m_AnimateOutTrigger");
            this.m_OnActivate = this.serializedObject.FindProperty("onActivate");
            this.m_OnDeactivate = this.serializedObject.FindProperty("onDeactivate");
            this.m_FirstSelected = this.serializedObject.FindProperty("m_FirstSelected");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            EditorGUILayout.Separator();
            this.DrawGeneralProperties();
            EditorGUILayout.Separator();
            this.DrawTransitionProperties();
            EditorGUILayout.Separator();
            this.DrawNavigationProperties();
            EditorGUILayout.Separator();
            this.DrawEventsProperties();
            EditorGUILayout.Separator();
            serializedObject.ApplyModifiedProperties();
        }

        protected void DrawNavigationProperties()
        {
            EditorGUILayout.LabelField("Navigation Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

            EditorGUILayout.PropertyField(this.m_FirstSelected, new GUIContent("First Selected"));

            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }

        protected void DrawGeneralProperties()
        {
            EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

            EditorGUILayout.PropertyField(this.m_Id, new GUIContent("ID"));
            EditorGUILayout.PropertyField(this.m_IsActivated, new GUIContent("Is Activated", "Whether the scene is active or not."));
            EditorGUILayout.PropertyField(this.m_Type, new GUIContent("Type"));

            UIScene.Type type = (UIScene.Type)this.m_Type.enumValueIndex;

            if (type == UIScene.Type.Preloaded)
            {
                EditorGUILayout.PropertyField(this.m_Content, new GUIContent("Content"));
            }
            else if (type == UIScene.Type.Prefab)
            {
                EditorGUILayout.PropertyField(this.m_Prefab, new GUIContent("Prefab"));
            }
            else if (type == UIScene.Type.Resource)
            {
                EditorGUILayout.PropertyField(this.m_Resource, new GUIContent("Resource"));
            }

            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }

        protected void DrawTransitionProperties()
        {
            EditorGUILayout.LabelField("Transition Properties", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

            EditorGUILayout.PropertyField(this.m_Transition, new GUIContent("Transition"));

            // Get the transition
            UIScene.Transition transition = (UIScene.Transition)this.m_Transition.enumValueIndex;

            if (transition != UIScene.Transition.None && transition != UIScene.Transition.Animation)
            {
                EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
                EditorGUILayout.PropertyField(this.m_TransitionDuration, new GUIContent("Duration"));
                EditorGUILayout.PropertyField(this.m_TransitionEasing, new GUIContent("Easing"));
                EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
            }
            else if (transition == UIScene.Transition.Animation)
            {
                EditorGUILayout.PropertyField(this.m_AnimateInTrigger, new GUIContent("Animate In Trigger"));
                EditorGUILayout.PropertyField(this.m_AnimateOutTrigger, new GUIContent("Animate Out Trigger"));

                UIScene scene = (target as UIScene);
                Animator animator = scene.animator;

                if (animator == null || animator.runtimeAnimatorController == null)
                {
                    Rect controlRect = EditorGUILayout.GetControlRect();
                    controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);

                    if (GUI.Button(controlRect, "Auto Generate Animation", EditorStyles.miniButton))
                    {
                        System.Collections.Generic.List<string> triggersList = new System.Collections.Generic.List<string>();
                        triggersList.Add(this.m_AnimateInTrigger.stringValue);
                        triggersList.Add(this.m_AnimateOutTrigger.stringValue);

                        // Generate the animator controller
                        UnityEditor.Animations.AnimatorController animatorController = UIAnimatorControllerGenerator.GenerateAnimatorContoller(triggersList, scene.gameObject.name, true);

                        if (animatorController != null)
                        {
                            if (animator == null)
                            {
                                animator = scene.gameObject.AddComponent<Animator>();
                            }
                            UnityEditor.Animations.AnimatorController.SetAnimatorController(animator, animatorController);
                        }
                    }
                }
            }

            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }

        protected void DrawEventsProperties()
        {
            EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
            EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);

            EditorGUILayout.PropertyField(this.m_OnActivate, new GUIContent("On Activate"));
            EditorGUILayout.PropertyField(this.m_OnDeactivate, new GUIContent("On Deactivate"));

            EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
        }
    }
}
