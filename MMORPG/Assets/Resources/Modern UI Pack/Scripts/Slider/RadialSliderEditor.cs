#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(RadialSlider))]
    public class RadialSliderEditor : Editor
    {
        private GUISkin customSkin;
        private RadialSlider rsTarget;
        private UIManagerSlider tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            rsTarget = (RadialSlider)target;

            try { tempUIM = rsTarget.GetComponent<UIManagerSlider>(); }
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

            var currentValue = serializedObject.FindProperty("currentValue");
            var onValueChanged = serializedObject.FindProperty("onValueChanged");
            var onPointerEnter = serializedObject.FindProperty("onPointerEnter");
            var onPointerExit = serializedObject.FindProperty("onPointerExit");
            var sliderImage = serializedObject.FindProperty("sliderImage");
            var indicatorPivot = serializedObject.FindProperty("indicatorPivot");
            var valueText = serializedObject.FindProperty("valueText");
            var rememberValue = serializedObject.FindProperty("rememberValue");
            var sliderTag = serializedObject.FindProperty("sliderTag");
            var minValue = serializedObject.FindProperty("minValue");
            var maxValue = serializedObject.FindProperty("maxValue");
            var isPercent = serializedObject.FindProperty("isPercent");
            var decimals = serializedObject.FindProperty("decimals");
            var contentTransform = serializedObject.FindProperty("contentTransform");
            var startPoint = serializedObject.FindProperty("startPoint");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Current Value"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                    currentValue.floatValue = EditorGUILayout.Slider(currentValue.floatValue, rsTarget.minValue, rsTarget.maxValue);

                    GUILayout.EndHorizontal();

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    EditorGUILayout.PropertyField(onPointerEnter, new GUIContent("On Pointer Enter"), true);
                    EditorGUILayout.PropertyField(onPointerExit, new GUIContent("On Pointer Exit"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(sliderImage, customSkin, "Slider Image");
                    MUIPEditorHandler.DrawProperty(indicatorPivot, customSkin, "Indicator Pivot");
                    MUIPEditorHandler.DrawProperty(valueText, customSkin, "Indicator Text");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    MUIPEditorHandler.DrawProperty(minValue, customSkin, "Min Value");
                    MUIPEditorHandler.DrawProperty(maxValue, customSkin, "Max Value");
                    MUIPEditorHandler.DrawProperty(decimals, customSkin, "Decimals");
                    isPercent.boolValue = MUIPEditorHandler.DrawToggle(isPercent.boolValue, customSkin, "Is Percent");
                    rememberValue.boolValue = MUIPEditorHandler.DrawToggle(rememberValue.boolValue, customSkin, "Save Value");

                    if (rememberValue.boolValue == true)
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

                        if (GUILayout.Button("Open UI Manager", customSkin.button))
                            EditorApplication.ExecuteMenuItem("Tools/Modern UI Pack/Show UI Manager");

                        if (GUILayout.Button("Disable UI Manager Connection", customSkin.button))
                        {
                            if (EditorUtility.DisplayDialog("Modern UI Pack", "Are you sure you want to disable UI Manager connection with the object? " +
                                "This operation cannot be undone.", "Yes", "Cancel"))
                            {
                                try { DestroyImmediate(tempUIM); }
                                catch { Debug.LogError("<b>[Pie Chart]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null) { MUIPEditorHandler.DrawUIManagerDisconnectedHeader(); }

                    break;
            }

            if (rsTarget.sliderImage != null && rsTarget.indicatorPivot != null && rsTarget.valueText != null)
            {
                rsTarget.SliderValueRaw = currentValue.floatValue;
                float normalizedAngle = rsTarget.SliderAngle / 360.0f;
                rsTarget.indicatorPivot.transform.localEulerAngles = new Vector3(180.0f, 0.0f, rsTarget.SliderAngle);
                rsTarget.sliderImage.fillAmount = normalizedAngle;
                rsTarget.valueText.text = string.Format("{0}{1}", currentValue.floatValue, rsTarget.isPercent ? "%" : "");
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif