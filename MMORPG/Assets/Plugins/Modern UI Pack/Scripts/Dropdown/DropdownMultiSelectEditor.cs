#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(DropdownMultiSelect))]
    public class DropdownMultiSelectEditor : Editor
    {
        private GUISkin customSkin;
        private DropdownMultiSelect dTarget;
        private UIManagerDropdown tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            dTarget = (DropdownMultiSelect)target;

            try { tempUIM = dTarget.GetComponent<UIManagerDropdown>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Dropdown Top Header");

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

            var items = serializedObject.FindProperty("items");
            var triggerObject = serializedObject.FindProperty("triggerObject");
            var itemParent = serializedObject.FindProperty("itemParent");
            var itemObject = serializedObject.FindProperty("itemObject");
            var scrollbar = serializedObject.FindProperty("scrollbar");
            var listParent = serializedObject.FindProperty("listParent");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableTrigger = serializedObject.FindProperty("enableTrigger");
            var enableScrollbar = serializedObject.FindProperty("enableScrollbar");
            var setHighPriority = serializedObject.FindProperty("setHighPriority");
            var outOnPointerExit = serializedObject.FindProperty("outOnPointerExit");
            var isListItem = serializedObject.FindProperty("isListItem");
            var invokeAtStart = serializedObject.FindProperty("invokeAtStart");
            var animationType = serializedObject.FindProperty("animationType");
            var itemSpacing = serializedObject.FindProperty("itemSpacing");
            var itemPaddingLeft = serializedObject.FindProperty("itemPaddingLeft");
            var itemPaddingRight = serializedObject.FindProperty("itemPaddingRight");
            var itemPaddingTop = serializedObject.FindProperty("itemPaddingTop");
            var itemPaddingBottom = serializedObject.FindProperty("itemPaddingBottom");
            var initAtStart = serializedObject.FindProperty("initAtStart");
            var transitionSmoothness = serializedObject.FindProperty("transitionSmoothness");
            var sizeSmoothness = serializedObject.FindProperty("sizeSmoothness");
            var panelSize = serializedObject.FindProperty("panelSize");
            var listRect = serializedObject.FindProperty("listRect");
            var listCG = serializedObject.FindProperty("listCG");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(items, new GUIContent("Dropdown Items"), true); 

                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(triggerObject, customSkin, "Trigger Object");
                    MUIPEditorHandler.DrawProperty(itemObject, customSkin, "Item Prefab");
                    MUIPEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    MUIPEditorHandler.DrawProperty(scrollbar, customSkin, "Scrollbar");
                    MUIPEditorHandler.DrawProperty(listParent, customSkin, "List Parent");

                    if (dTarget.animationType == DropdownMultiSelect.AnimationType.Modular)
                    {
                        MUIPEditorHandler.DrawProperty(listRect, customSkin, "List Rect");
                        MUIPEditorHandler.DrawProperty(listCG, customSkin, "List Canvas Group");
                    }

                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 6);
                    enableIcon.boolValue = MUIPEditorHandler.DrawToggle(enableIcon.boolValue, customSkin, "Enable Header Icon");
                    enableScrollbar.boolValue = MUIPEditorHandler.DrawToggle(enableScrollbar.boolValue, customSkin, "Enable Scrollbar");
                    MUIPEditorHandler.DrawPropertyCW(itemSpacing, customSkin, "Item Spacing", 90);

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(new GUIContent("Item Padding"), customSkin.FindStyle("Text"), GUILayout.Width(90));
                    GUILayout.EndHorizontal();
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(itemPaddingTop, new GUIContent("Top"));
                    EditorGUILayout.PropertyField(itemPaddingBottom, new GUIContent("Bottom"));
                    EditorGUILayout.PropertyField(itemPaddingLeft, new GUIContent("Left"));
                    EditorGUILayout.PropertyField(itemPaddingRight, new GUIContent("Right"));

                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Animation Header", 10);
                    MUIPEditorHandler.DrawProperty(animationType, customSkin, "Animation Type");

                    if (dTarget.animationType == DropdownMultiSelect.AnimationType.Modular)
                    {
                        MUIPEditorHandler.DrawProperty(transitionSmoothness, customSkin, "Transition Speed");
                        MUIPEditorHandler.DrawProperty(sizeSmoothness, customSkin, "Size Smoothness");
                        MUIPEditorHandler.DrawProperty(panelSize, customSkin, "Panel Size");
                    }

                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 10);
                    initAtStart.boolValue = MUIPEditorHandler.DrawToggle(initAtStart.boolValue, customSkin, "Initialize At Start");
                    invokeAtStart.boolValue = MUIPEditorHandler.DrawToggle(invokeAtStart.boolValue, customSkin, "Invoke At Start");
                    enableTrigger.boolValue = MUIPEditorHandler.DrawToggle(enableTrigger.boolValue, customSkin, "Enable Trigger");

                    if (enableTrigger.boolValue == true && dTarget.triggerObject == null)
                    {
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.HelpBox("'Trigger Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                        GUILayout.EndHorizontal();
                    }

                    setHighPriority.boolValue = MUIPEditorHandler.DrawToggle(setHighPriority.boolValue, customSkin, "Set High Priority");
                    outOnPointerExit.boolValue = MUIPEditorHandler.DrawToggle(outOnPointerExit.boolValue, customSkin, "Out On Pointer Exit");
                   
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    isListItem.boolValue = MUIPEditorHandler.DrawTogglePlain(isListItem.boolValue, customSkin, "Is List Item");

                    GUILayout.Space(3);

                    if (isListItem.boolValue == true)
                    {
                        MUIPEditorHandler.DrawPropertyPlain(listParent, customSkin, "List Parent");

                        if (dTarget.listParent == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'List Parent' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null)
                    {
                        MUIPEditorHandler.DrawUIManagerConnectedHeader();
                        tempUIM.overrideColors = MUIPEditorHandler.DrawToggle(tempUIM.overrideColors, customSkin, "Override Colors");
                        tempUIM.overrideFonts = MUIPEditorHandler.DrawToggle(tempUIM.overrideFonts, customSkin, "Override Fonts");

                        if (GUILayout.Button("Open UI Manager", customSkin.button))
                            EditorApplication.ExecuteMenuItem("Tools/Modern UI Pack/Show UI Manager");

                        if (GUILayout.Button("Disable UI Manager Connection", customSkin.button))
                        {
                            if (EditorUtility.DisplayDialog("Modern UI Pack", "Are you sure you want to disable UI Manager connection with the object? " +
                                "This operation cannot be undone.", "Yes", "Cancel"))
                            {
                                try { DestroyImmediate(tempUIM); }
                                catch { Debug.LogError("<b>[Dropdown]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null) { MUIPEditorHandler.DrawUIManagerDisconnectedHeader(); }

                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif