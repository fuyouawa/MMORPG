#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(ContextMenuContent))]
    public class ContextMenuContentEditor : Editor
    {
        private GUISkin customSkin;
        private ContextMenuContent cmcTarget;
        private int currentTab;

        private void OnEnable()
        {
            cmcTarget = (ContextMenuContent)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "CM Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                currentTab = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 2;

            GUILayout.EndHorizontal();

            var contextManager = serializedObject.FindProperty("contextManager");
            var itemParent = serializedObject.FindProperty("itemParent");
            var contexItems = serializedObject.FindProperty("contexItems");
            var useIn3D = serializedObject.FindProperty("useIn3D");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
#if UNITY_2020_1_OR_NEWER
                    GUILayout.BeginVertical();
#else
                    GUILayout.BeginVertical(EditorStyles.helpBox);
#endif
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(contexItems, new GUIContent("Context Menu Items"), true);
                    contexItems.isExpanded = true;

                    EditorGUI.indentLevel = 0;

                    if (GUILayout.Button("+  Add a new item", customSkin.button))
                        cmcTarget.AddNewItem();

                    GUILayout.EndVertical();
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(contextManager, customSkin, "Context Manager");
                    MUIPEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    useIn3D.boolValue = MUIPEditorHandler.DrawToggle(useIn3D.boolValue, customSkin, "Use In 3D");
                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif