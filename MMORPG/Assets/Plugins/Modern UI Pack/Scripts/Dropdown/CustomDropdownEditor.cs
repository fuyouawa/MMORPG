#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(CustomDropdown))]
    public class CustomDropdownEditor : Editor
    {
        private GUISkin customSkin;
        private CustomDropdown dTarget;
        private UIManagerDropdown tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            dTarget = (CustomDropdown)target;

            try { tempUIM = dTarget.GetComponent<UIManagerDropdown>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }

            if (dTarget.selectedItemIndex > dTarget.items.Count - 1) { dTarget.selectedItemIndex = 0; }
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
            var onValueChanged = serializedObject.FindProperty("onValueChanged");
            var onItemTextChanged = serializedObject.FindProperty("onItemTextChanged");

            var triggerObject = serializedObject.FindProperty("triggerObject");
            var selectedText = serializedObject.FindProperty("selectedText");
            var selectedImage = serializedObject.FindProperty("selectedImage");
            var itemParent = serializedObject.FindProperty("itemParent");
            var itemObject = serializedObject.FindProperty("itemObject");
            var scrollbar = serializedObject.FindProperty("scrollbar");
            var listParent = serializedObject.FindProperty("listParent");
            var listRect = serializedObject.FindProperty("listRect");
            var listCG = serializedObject.FindProperty("listCG");

            var animationType = serializedObject.FindProperty("animationType");
            var panelDirection = serializedObject.FindProperty("panelDirection");
            var panelSize = serializedObject.FindProperty("panelSize");
            var curveSpeed = serializedObject.FindProperty("curveSpeed");
            var animationCurve = serializedObject.FindProperty("animationCurve");

            var saveSelected = serializedObject.FindProperty("saveSelected");
            var saveKey = serializedObject.FindProperty("saveKey");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableTrigger = serializedObject.FindProperty("enableTrigger");
            var enableScrollbar = serializedObject.FindProperty("enableScrollbar");
            var outOnPointerExit = serializedObject.FindProperty("outOnPointerExit");
            var setHighPriority = serializedObject.FindProperty("setHighPriority");
            var invokeAtStart = serializedObject.FindProperty("invokeAtStart");
            var initAtStart = serializedObject.FindProperty("initAtStart");
            var selectedItemIndex = serializedObject.FindProperty("selectedItemIndex");
            var enableDropdownSounds = serializedObject.FindProperty("enableDropdownSounds");
            var useHoverSound = serializedObject.FindProperty("useHoverSound");
            var useClickSound = serializedObject.FindProperty("useClickSound");
            var hoverSound = serializedObject.FindProperty("hoverSound");
            var clickSound = serializedObject.FindProperty("clickSound");
            var soundSource = serializedObject.FindProperty("soundSource");
            var itemSpacing = serializedObject.FindProperty("itemSpacing");
            var itemPaddingLeft = serializedObject.FindProperty("itemPaddingLeft");
            var itemPaddingRight = serializedObject.FindProperty("itemPaddingRight");
            var itemPaddingTop = serializedObject.FindProperty("itemPaddingTop");
            var itemPaddingBottom = serializedObject.FindProperty("itemPaddingBottom");
            var extendEvents = serializedObject.FindProperty("extendEvents");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (Application.isPlaying == false && dTarget.items.Count != 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();

                        GUI.enabled = false;
                        EditorGUILayout.LabelField(new GUIContent("Selected Item:"), customSkin.FindStyle("Text"), GUILayout.Width(78));
                        GUI.enabled = true;

                        EditorGUILayout.LabelField(new GUIContent(dTarget.items[selectedItemIndex.intValue].itemName), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        selectedItemIndex.intValue = EditorGUILayout.IntSlider(selectedItemIndex.intValue, 0, dTarget.items.Count - 1);

                        GUILayout.EndVertical();
                    }

                    else if (Application.isPlaying == true && dTarget.items.Count != 0)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();
                        GUI.enabled = false;

                        EditorGUILayout.LabelField(new GUIContent("Current Item:"), customSkin.FindStyle("Text"), GUILayout.Width(74));
                        EditorGUILayout.LabelField(new GUIContent(dTarget.items[dTarget.selectedItemIndex].itemName), customSkin.FindStyle("Text"));

                        GUILayout.EndHorizontal();
                        GUILayout.Space(2);

                        EditorGUILayout.IntSlider(dTarget.index, 0, dTarget.items.Count - 1);

                        GUI.enabled = true;
                        GUILayout.EndVertical();
                    }

                    else { EditorGUILayout.HelpBox("There is no item in the list.", MessageType.Warning); }

                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(items, new GUIContent("Dropdown Items"), true);          
                    EditorGUI.indentLevel = 0;
                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);

                    if (extendEvents.boolValue == true)
                    {
                        EditorGUILayout.PropertyField(onItemTextChanged, new GUIContent("On Item Text Changed"), true);
                    }
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(triggerObject, customSkin, "Trigger Object");
                    MUIPEditorHandler.DrawProperty(selectedText, customSkin, "Selected Text");
                    MUIPEditorHandler.DrawProperty(selectedImage, customSkin, "Selected Image");
                    MUIPEditorHandler.DrawProperty(itemObject, customSkin, "Item Prefab");
                    MUIPEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    MUIPEditorHandler.DrawProperty(scrollbar, customSkin, "Scrollbar");

                    if (dTarget.animationType == CustomDropdown.AnimationType.Modular)
                    {
                        MUIPEditorHandler.DrawProperty(listRect, customSkin, "List Rect");
                        MUIPEditorHandler.DrawProperty(listCG, customSkin, "List Canvas Group");
                    }

                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 6);
                    enableIcon.boolValue = MUIPEditorHandler.DrawToggle(enableIcon.boolValue, customSkin, "Enable Header Icon");
                    enableScrollbar.boolValue = MUIPEditorHandler.DrawToggle(enableScrollbar.boolValue, customSkin, "Enable Scrollbar");
                    extendEvents.boolValue = MUIPEditorHandler.DrawToggle(extendEvents.boolValue, customSkin, "Extend Events");
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

                    if (dTarget.animationType == CustomDropdown.AnimationType.Modular)
                    {
                        // MUIPEditorHandler.DrawProperty(panelDirection, customSkin, "Panel Direction");
                        MUIPEditorHandler.DrawProperty(panelSize, customSkin, "Panel Size");
                        MUIPEditorHandler.DrawProperty(curveSpeed, customSkin, "Curve Speed");
                        MUIPEditorHandler.DrawProperty(animationCurve, customSkin, "Animation Curve");
                    }

                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 10);
                    initAtStart.boolValue = MUIPEditorHandler.DrawToggle(initAtStart.boolValue, customSkin, "Initialize At Start");
                    invokeAtStart.boolValue = MUIPEditorHandler.DrawToggle(invokeAtStart.boolValue, customSkin, "Invoke At Start");

                    if (dTarget.selectedImage != null)
                    {
                        if (enableIcon.boolValue == true) { dTarget.selectedImage.enabled = true; }
                        else { dTarget.selectedImage.enabled = false; }
                    }

                    else
                    {
                        if (enableIcon.boolValue == true)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Selected Image' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }                       
                    }

                    enableTrigger.boolValue = MUIPEditorHandler.DrawToggle(enableTrigger.boolValue, customSkin, "Enable Trigger");
                    if (enableTrigger.boolValue == true && dTarget.triggerObject == null) { EditorGUILayout.HelpBox("'Trigger Object' is missing from the resources.", MessageType.Warning); }

                    setHighPriority.boolValue = MUIPEditorHandler.DrawToggle(setHighPriority.boolValue, customSkin, "Set High Priority");
                    if (setHighPriority.boolValue == true) { EditorGUILayout.HelpBox("Set High Priority; renders the content above all objects when the dropdown is open.", MessageType.Info); }

                    outOnPointerExit.boolValue = MUIPEditorHandler.DrawToggle(outOnPointerExit.boolValue, customSkin, "Out On Pointer Exit");
                   
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                   
                    enableDropdownSounds.boolValue = MUIPEditorHandler.DrawTogglePlain(enableDropdownSounds.boolValue, customSkin, "Enable Dropdown Sounds");
                   
                    GUILayout.Space(3);

                    if (enableDropdownSounds.boolValue == true)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Space(-3);

                        useHoverSound.boolValue = MUIPEditorHandler.DrawTogglePlain(useHoverSound.boolValue, customSkin, "Enable Hover Sound");
                     
                        GUILayout.Space(3);

                        if (useHoverSound.boolValue == true)
                            MUIPEditorHandler.DrawProperty(hoverSound, customSkin, "Hover Sound");

                        GUILayout.EndVertical();
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Space(-3);
                        
                        useClickSound.boolValue = MUIPEditorHandler.DrawTogglePlain(useClickSound.boolValue, customSkin, "Enable Click Sound");
                       
                        GUILayout.Space(3);

                        if (useClickSound.boolValue == true)
                            MUIPEditorHandler.DrawProperty(clickSound, customSkin, "Click Sound");

                        GUILayout.EndVertical();

                        MUIPEditorHandler.DrawProperty(soundSource, customSkin, "Sound Source");

                        if (dTarget.soundSource == null)
                        {
                            EditorGUILayout.HelpBox("'Sound Source' is not assigned. Go to Resources tab or click the button to create a new audio source.", MessageType.Warning);

                            if (GUILayout.Button("+ Create a new one", customSkin.button))
                            {
                                dTarget.soundSource = dTarget.gameObject.AddComponent(typeof(AudioSource)) as AudioSource;
                                currentTab = 2;
                            }
                        }
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    saveSelected.boolValue = MUIPEditorHandler.DrawTogglePlain(saveSelected.boolValue, customSkin, "Save Selected");
                  
                    GUILayout.Space(3);

                    if (saveSelected.boolValue == true)
                    {
                        MUIPEditorHandler.DrawPropertyPlainCW(saveKey, customSkin, "Save Key:", 90);
                        EditorGUILayout.HelpBox("Each dropdown should has its own save key.", MessageType.Info);
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