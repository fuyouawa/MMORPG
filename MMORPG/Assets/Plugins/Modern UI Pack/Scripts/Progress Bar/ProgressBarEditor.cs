#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(ProgressBar))]
    public class ProgressBarEditor : Editor
    {
        private GUISkin customSkin;
        private ProgressBar pbTarget;
        private UIManagerProgressBar tempUIM;
        private UIManagerProgressBarLoop tempFilledUIM;
        private int currentTab;

        private void OnEnable()
        {
            pbTarget = (ProgressBar)target;

            try
            {
                if (pbTarget.isLooped == false) { tempUIM = pbTarget.GetComponent<UIManagerProgressBar>(); }
                else { tempFilledUIM = pbTarget.GetComponent<UIManagerProgressBarLoop>(); }
            }

            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "PB Top Header");

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

            var currentPercent = serializedObject.FindProperty("currentPercent");
            var speed = serializedObject.FindProperty("speed");
            var minValue = serializedObject.FindProperty("minValue");
            var maxValue = serializedObject.FindProperty("maxValue");
            var valueLimit = serializedObject.FindProperty("valueLimit");
            var loadingBar = serializedObject.FindProperty("loadingBar");
            var textPercent = serializedObject.FindProperty("textPercent");
            var isOn = serializedObject.FindProperty("isOn");
            var restart = serializedObject.FindProperty("restart");
            var invert = serializedObject.FindProperty("invert");
            var addPrefix = serializedObject.FindProperty("addPrefix");
            var addSuffix = serializedObject.FindProperty("addSuffix");
            var prefix = serializedObject.FindProperty("prefix");
            var suffix = serializedObject.FindProperty("suffix");
            var decimals = serializedObject.FindProperty("decimals");
            var onValueChanged = serializedObject.FindProperty("onValueChanged");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Current Percent"), customSkin.FindStyle("Text"), GUILayout.Width(100));
                    pbTarget.currentPercent = EditorGUILayout.Slider(pbTarget.currentPercent, minValue.floatValue, maxValue.floatValue);
                    currentPercent.floatValue = pbTarget.currentPercent;
                 
                    GUILayout.EndHorizontal();

                    if (pbTarget.loadingBar != null && pbTarget.textPercent != null) { pbTarget.UpdateUI(); }
                    else
                    {
                        if (pbTarget.loadingBar == null || pbTarget.textPercent == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("One or more resources needs to be assigned.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField(new GUIContent("Min / Max Value"), customSkin.FindStyle("Text"), GUILayout.Width(110));
                    GUILayout.BeginHorizontal();
                    GUILayout.Space(2);

                    minValue.floatValue = EditorGUILayout.Slider(minValue.floatValue, 0, maxValue.floatValue - 1);
                    maxValue.floatValue = EditorGUILayout.Slider(maxValue.floatValue, minValue.floatValue + 1, valueLimit.floatValue);

                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    EditorGUILayout.HelpBox("You can increase the max value limit by changing 'Value Limit' in the settings tab.", MessageType.Info);
                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"));
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(loadingBar, customSkin, "Loading Bar");
                    MUIPEditorHandler.DrawProperty(textPercent, customSkin, "Text Indicator");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    isOn.boolValue = MUIPEditorHandler.DrawToggle(isOn.boolValue, customSkin, "Is On");
                    restart.boolValue = MUIPEditorHandler.DrawToggle(restart.boolValue, customSkin, "Restart / Loop");
                    invert.boolValue = MUIPEditorHandler.DrawToggle(invert.boolValue, customSkin, "Invert");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    addPrefix.boolValue = MUIPEditorHandler.DrawTogglePlain(addPrefix.boolValue, customSkin, "Add Prefix");
                    GUILayout.Space(3);

                    if (addPrefix.boolValue == true)
                        MUIPEditorHandler.DrawPropertyPlainCW(prefix, customSkin, "Prefix:", 40);

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    addSuffix.boolValue = MUIPEditorHandler.DrawTogglePlain(addSuffix.boolValue, customSkin, "Add Suffix");
                    GUILayout.Space(3);

                    if (addSuffix.boolValue == true)
                        MUIPEditorHandler.DrawPropertyPlainCW(suffix, customSkin, "Suffix:", 40);

                    GUILayout.EndVertical();
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Value Limit"), customSkin.FindStyle("Text"), GUILayout.Width(80));
                    EditorGUILayout.PropertyField(valueLimit, new GUIContent(""));
                   
                    if (valueLimit.floatValue <= minValue.floatValue) { valueLimit.floatValue = minValue.floatValue + 1; }

                    GUILayout.EndHorizontal();
                    MUIPEditorHandler.DrawPropertyCW(decimals, customSkin, "Decimals", 80);
                    MUIPEditorHandler.DrawPropertyCW(speed, customSkin, "Speed", 80);

                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null && pbTarget.isLooped == false)
                    {
                        EditorGUILayout.HelpBox("This object is connected with UI Manager. Some parameters (such as colors, " +
                            "fonts or booleans) are managed by the manager.", MessageType.Info);

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
                                catch { Debug.LogError("<b>[Progress Bar]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempFilledUIM != null && pbTarget.isLooped == true)
                    {
                        MUIPEditorHandler.DrawUIManagerConnectedHeader();

                        tempUIM.overrideColors = MUIPEditorHandler.DrawToggle(tempUIM.overrideColors, customSkin, "Override Colors");

                        if (GUILayout.Button("Open UI Manager", customSkin.button))
                            EditorApplication.ExecuteMenuItem("Tools/Modern UI Pack/Show UI Manager");

                        if (GUILayout.Button("Disable UI Manager Connection", customSkin.button))
                        {
                            if (EditorUtility.DisplayDialog("Modern UI Pack", "Are you sure you want to disable UI Manager connection with the object? " +
                                "This operation cannot be undone.", "Yes", "Cancel"))
                            {
                                try { DestroyImmediate(tempUIM); }
                                catch { Debug.LogError("<b>[Progress Bar]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null && tempFilledUIM == null) { MUIPEditorHandler.DrawUIManagerDisconnectedHeader(); }

                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif