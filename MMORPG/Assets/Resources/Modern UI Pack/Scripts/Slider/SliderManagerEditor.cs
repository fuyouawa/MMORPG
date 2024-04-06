#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(SliderManager))]
    public class SliderManagerEditor : Editor
    {
        private GUISkin customSkin;
        private SliderManager sTarget;
        private UIManagerSlider tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            sTarget = (SliderManager)target;

            try { tempUIM = sTarget.GetComponent<UIManagerSlider>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Slider Top Header");

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

            var sliderEvent = serializedObject.FindProperty("sliderEvent");
            var sliderObject = serializedObject.FindProperty("mainSlider");
            var valueText = serializedObject.FindProperty("valueText");
            var popupValueText = serializedObject.FindProperty("popupValueText");
            var enableSaving = serializedObject.FindProperty("enableSaving");
            var sliderTag = serializedObject.FindProperty("sliderTag");
            var usePercent = serializedObject.FindProperty("usePercent");
            var useRoundValue = serializedObject.FindProperty("useRoundValue");
            var showValue = serializedObject.FindProperty("showValue");
            var showPopupValue = serializedObject.FindProperty("showPopupValue");
            var minValue = serializedObject.FindProperty("minValue");
            var maxValue = serializedObject.FindProperty("maxValue");
            var invokeOnAwake = serializedObject.FindProperty("invokeOnAwake");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (sTarget.mainSlider != null)
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        EditorGUILayout.LabelField(new GUIContent("Current Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        sTarget.mainSlider.value = EditorGUILayout.Slider(sTarget.mainSlider.value, sTarget.mainSlider.minValue, sTarget.mainSlider.maxValue);

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        EditorGUILayout.LabelField(new GUIContent("Min Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));

                        if (sTarget.mainSlider.wholeNumbers == false)
                            sTarget.mainSlider.minValue = EditorGUILayout.FloatField(sTarget.mainSlider.minValue);
                        else
                            sTarget.mainSlider.minValue = EditorGUILayout.IntField((int)sTarget.mainSlider.minValue);

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        EditorGUILayout.LabelField(new GUIContent("Max Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));

                        if (sTarget.mainSlider.wholeNumbers == false)
                            sTarget.mainSlider.maxValue = EditorGUILayout.FloatField(sTarget.mainSlider.maxValue);
                        else
                            sTarget.mainSlider.maxValue = EditorGUILayout.IntField((int)sTarget.mainSlider.maxValue);

                        GUILayout.EndHorizontal();
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);

                        sTarget.mainSlider.wholeNumbers = GUILayout.Toggle(sTarget.mainSlider.wholeNumbers, new GUIContent("Use Whole Numbers"), customSkin.FindStyle("Toggle"));
                        sTarget.mainSlider.wholeNumbers = GUILayout.Toggle(sTarget.mainSlider.wholeNumbers, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));

                        GUILayout.EndHorizontal();
                    }

                    else
                        EditorGUILayout.HelpBox("'Main Slider' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(sliderEvent, new GUIContent("On Value Changed"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(sliderObject, customSkin, "Slider Source");
                    if (showValue.boolValue == true) { MUIPEditorHandler.DrawProperty(valueText, customSkin, "Label Text"); }
                    if (showPopupValue.boolValue == true) { MUIPEditorHandler.DrawProperty(popupValueText, customSkin, "Popup Label Text"); }
                    GUILayout.Space(4);
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    usePercent.boolValue = MUIPEditorHandler.DrawToggle(usePercent.boolValue, customSkin, "Use Percent");
                    showValue.boolValue = MUIPEditorHandler.DrawToggle(showValue.boolValue, customSkin, "Show Label");
                    showPopupValue.boolValue = MUIPEditorHandler.DrawToggle(showPopupValue.boolValue, customSkin, "Show Popup Label");
                    useRoundValue.boolValue = MUIPEditorHandler.DrawToggle(useRoundValue.boolValue, customSkin, "Use Round Value");
                    invokeOnAwake.boolValue = MUIPEditorHandler.DrawToggle(invokeOnAwake.boolValue, customSkin, "Invoke On Awake");
                    enableSaving.boolValue = MUIPEditorHandler.DrawToggle(enableSaving.boolValue, customSkin, "Save Value");

                    if (enableSaving.boolValue == true)
                    {
                        EditorGUI.indentLevel = 2;
                        MUIPEditorHandler.DrawPropertyPlainCW(sliderTag, customSkin, "Tag:", 40);
                        EditorGUI.indentLevel = 0;
                        GUILayout.Space(2);
                        EditorGUILayout.HelpBox("Each slider should has its own unique tag.", MessageType.Info);
                    }

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
                                catch { Debug.LogError("<b>[Horizontal Selector]</b> Failed to delete UI Manager connection.", this); }
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