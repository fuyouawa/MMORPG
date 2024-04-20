#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using static Michsky.MUIP.ButtonManager;

namespace Michsky.MUIP
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonManager))]
    public class ButtonManagerEditor : Editor
    {
        private GUISkin customSkin;
        private ButtonManager buttonTarget;
        private UIManagerButton tempUIM;

        private void OnEnable()
        {
            buttonTarget = (ButtonManager)target;

            try { tempUIM = buttonTarget.GetComponent<UIManagerButton>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Button Top Header");

            GUIContent[] toolbarTabs = new GUIContent[3];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Resources");
            toolbarTabs[2] = new GUIContent("Settings");

            buttonTarget.latestTabIndex = MUIPEditorHandler.DrawTabs(buttonTarget.latestTabIndex, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                buttonTarget.latestTabIndex = 0;
            if (GUILayout.Button(new GUIContent("Resources", "Resources"), customSkin.FindStyle("Tab Resources")))
                buttonTarget.latestTabIndex = 1;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                buttonTarget.latestTabIndex = 2;

            GUILayout.EndHorizontal();

            var normalCG = serializedObject.FindProperty("normalCG");
            var highlightCG = serializedObject.FindProperty("highlightCG");
            var disabledCG = serializedObject.FindProperty("disabledCG");
            var normalText = serializedObject.FindProperty("normalText");
            var highlightedText = serializedObject.FindProperty("highlightedText");
            var disabledText = serializedObject.FindProperty("disabledText");
            var normalImageObj = serializedObject.FindProperty("normalImage");
            var highlightImageObj = serializedObject.FindProperty("highlightImage");
            var disabledImageObj = serializedObject.FindProperty("disabledImage");
            var rippleParent = serializedObject.FindProperty("rippleParent");
            var soundSource = serializedObject.FindProperty("soundSource");

            var buttonIcon = serializedObject.FindProperty("buttonIcon");
            var buttonText = serializedObject.FindProperty("buttonText");
            var iconScale = serializedObject.FindProperty("iconScale");
            var textSize = serializedObject.FindProperty("textSize");
            var hoverSound = serializedObject.FindProperty("hoverSound");
            var clickSound = serializedObject.FindProperty("clickSound");

            var autoFitContent = serializedObject.FindProperty("autoFitContent");
            var padding = serializedObject.FindProperty("padding");
            var spacing = serializedObject.FindProperty("spacing");
            var disabledLayout = serializedObject.FindProperty("disabledLayout");
            var normalLayout = serializedObject.FindProperty("normalLayout");
            var highlightedLayout = serializedObject.FindProperty("highlightedLayout");
            var mainLayout = serializedObject.FindProperty("mainLayout");
            var mainFitter = serializedObject.FindProperty("mainFitter");
            var targetFitter = serializedObject.FindProperty("targetFitter");
            var targetRect = serializedObject.FindProperty("targetRect");

            var isInteractable = serializedObject.FindProperty("isInteractable");
            var enableIcon = serializedObject.FindProperty("enableIcon");
            var enableText = serializedObject.FindProperty("enableText");
            var useCustomIconSize = serializedObject.FindProperty("useCustomIconSize");
            var useCustomTextSize = serializedObject.FindProperty("useCustomTextSize");
            var useUINavigation = serializedObject.FindProperty("useUINavigation");
            var navigationMode = serializedObject.FindProperty("navigationMode");
            var wrapAround = serializedObject.FindProperty("wrapAround");
            var selectOnUp = serializedObject.FindProperty("selectOnUp");
            var selectOnDown = serializedObject.FindProperty("selectOnDown");
            var selectOnLeft = serializedObject.FindProperty("selectOnLeft");
            var selectOnRight = serializedObject.FindProperty("selectOnRight");
            var checkForDoubleClick = serializedObject.FindProperty("checkForDoubleClick");
            var enableButtonSounds = serializedObject.FindProperty("enableButtonSounds");
            var useHoverSound = serializedObject.FindProperty("useHoverSound");
            var useClickSound = serializedObject.FindProperty("useClickSound");
            var useRipple = serializedObject.FindProperty("useRipple");
            var fadingMultiplier = serializedObject.FindProperty("fadingMultiplier");
            var doubleClickPeriod = serializedObject.FindProperty("doubleClickPeriod");
            var animationSolution = serializedObject.FindProperty("animationSolution");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");

            var renderOnTop = serializedObject.FindProperty("renderOnTop");
            var centered = serializedObject.FindProperty("centered");
            var rippleUpdateMode = serializedObject.FindProperty("rippleUpdateMode");
            var targetCanvas = serializedObject.FindProperty("targetCanvas");
            var rippleShape = serializedObject.FindProperty("rippleShape");
            var speed = serializedObject.FindProperty("speed");
            var maxSize = serializedObject.FindProperty("maxSize");
            var startColor = serializedObject.FindProperty("startColor");
            var transitionColor = serializedObject.FindProperty("transitionColor");

            var onClick = serializedObject.FindProperty("onClick");
            var onDoubleClick = serializedObject.FindProperty("onDoubleClick");
            var onHover = serializedObject.FindProperty("onHover");
            var onLeave = serializedObject.FindProperty("onLeave");

            switch (buttonTarget.latestTabIndex)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (useCustomContent.boolValue == false)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Space(-3);

                        enableIcon.boolValue = MUIPEditorHandler.DrawTogglePlain(enableIcon.boolValue, customSkin, "Enable Icon");

                        GUILayout.Space(4);

                        if (enableIcon.boolValue == true)
                        {
                            MUIPEditorHandler.DrawPropertyCW(buttonIcon, customSkin, "Button Icon", 80);
                            MUIPEditorHandler.DrawPropertyCW(iconScale, customSkin, "Icon Scale", 80);
                        }

                        GUILayout.EndVertical();

                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.Space(-3);

                        enableText.boolValue = MUIPEditorHandler.DrawTogglePlain(enableText.boolValue, customSkin, "Enable Text");

                        GUILayout.Space(4);

                        if (enableText.boolValue == true)
                        {
                            MUIPEditorHandler.DrawPropertyCW(buttonText, customSkin, "Button Text", 80);
                            if (useCustomTextSize.boolValue == false) { MUIPEditorHandler.DrawPropertyCW(textSize, customSkin, "Text Size", 80); }
                        }

                        GUILayout.EndVertical();
                        buttonTarget.UpdateUI();
                    }

                    else { EditorGUILayout.HelpBox("'Use Custom Content' is enabled. Content is now managed manually.", MessageType.Info); }

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    autoFitContent.boolValue = MUIPEditorHandler.DrawTogglePlain(autoFitContent.boolValue, customSkin, "Auto-Fit Content");

                    GUILayout.Space(4);

                    if (autoFitContent.boolValue == true)
                    {
                        GUILayout.BeginHorizontal(EditorStyles.helpBox);
                        EditorGUI.indentLevel = 1;
                        EditorGUILayout.PropertyField(padding, new GUIContent(" Padding"), true);
                        EditorGUI.indentLevel = 0;
                        GUILayout.EndHorizontal();
                        MUIPEditorHandler.DrawProperty(spacing, customSkin, "Spacing");
                    }

                    GUILayout.EndVertical();

                    if (Application.isPlaying == true && GUILayout.Button("Refresh", customSkin.button)) { buttonTarget.UpdateUI(); }

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onClick, new GUIContent("On Click"), true);
                    EditorGUILayout.PropertyField(onDoubleClick, new GUIContent("On Double Click"), true);
                    EditorGUILayout.PropertyField(onHover, new GUIContent("On Hover"), true);
                    EditorGUILayout.PropertyField(onLeave, new GUIContent("On Leave"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(normalCG, customSkin, "Normal CG");
                    MUIPEditorHandler.DrawProperty(highlightCG, customSkin, "Highlight CG");
                    MUIPEditorHandler.DrawProperty(disabledCG, customSkin, "Disabled CG");

                    if (enableText.boolValue == true)
                    {
                        MUIPEditorHandler.DrawProperty(normalText, customSkin, "Normal Text");
                        MUIPEditorHandler.DrawProperty(highlightedText, customSkin, "Highlighted Text");
                        MUIPEditorHandler.DrawProperty(disabledText, customSkin, "Disabled Text");
                    }

                    if (enableIcon.boolValue == true)
                    {
                        MUIPEditorHandler.DrawProperty(normalImageObj, customSkin, "Normal Icon");
                        MUIPEditorHandler.DrawProperty(highlightImageObj, customSkin, "Highlight Icon");
                        MUIPEditorHandler.DrawProperty(disabledImageObj, customSkin, "Disabled Icon");
                    }

                    MUIPEditorHandler.DrawProperty(disabledLayout, customSkin, "Disabled Layout");
                    MUIPEditorHandler.DrawProperty(normalLayout, customSkin, "Normal Layout");
                    MUIPEditorHandler.DrawProperty(highlightedLayout, customSkin, "Highlighted Layout");

                    if (autoFitContent.boolValue == true)
                    {
                        MUIPEditorHandler.DrawProperty(mainLayout, customSkin, "Main Layout");
                        MUIPEditorHandler.DrawProperty(mainFitter, customSkin, "Main Fitter");
                        MUIPEditorHandler.DrawProperty(targetFitter, customSkin, "Target Fitter");
                        MUIPEditorHandler.DrawProperty(targetRect, customSkin, "Target Rect");
                    }

                    if (useRipple.boolValue == true) { MUIPEditorHandler.DrawProperty(rippleParent, customSkin, "Ripple Parent"); }
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    MUIPEditorHandler.DrawProperty(animationSolution, customSkin, "Animation Solution");
                    MUIPEditorHandler.DrawProperty(fadingMultiplier, customSkin, "Fading Multiplier");
                    MUIPEditorHandler.DrawProperty(doubleClickPeriod, customSkin, "Double Click Period");
                    isInteractable.boolValue = MUIPEditorHandler.DrawToggle(isInteractable.boolValue, customSkin, "Is Interactable");
                    if (useCustomContent.boolValue == true || enableText.boolValue == false) { GUI.enabled = false; }
                    useCustomTextSize.boolValue = MUIPEditorHandler.DrawToggle(useCustomTextSize.boolValue, customSkin, "Use Custom Text Size");
                    GUI.enabled = true;
                    useCustomContent.boolValue = MUIPEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content");
                    checkForDoubleClick.boolValue = MUIPEditorHandler.DrawToggle(checkForDoubleClick.boolValue, customSkin, "Check For Double Click");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    useUINavigation.boolValue = MUIPEditorHandler.DrawTogglePlain(useUINavigation.boolValue, customSkin, "Use UI Navigation");

                    GUILayout.Space(4);

                    if (useUINavigation.boolValue == true)
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        MUIPEditorHandler.DrawPropertyPlain(navigationMode, customSkin, "Navigation Mode");

                        if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Horizontal) 
                        {
                            EditorGUI.indentLevel = 1;
                         //   GUILayout.Space(-3);
                            wrapAround.boolValue = MUIPEditorHandler.DrawToggle(wrapAround.boolValue, customSkin, "Wrap Around");
                          //  GUILayout.Space(4);
                            EditorGUI.indentLevel = 0;
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Vertical) 
                        { 
                            wrapAround.boolValue = MUIPEditorHandler.DrawTogglePlain(wrapAround.boolValue, customSkin, "Wrap Around");
                        }

                        else if (buttonTarget.navigationMode == UnityEngine.UI.Navigation.Mode.Explicit) 
                        {
                            EditorGUI.indentLevel = 1;
                            MUIPEditorHandler.DrawPropertyPlain(selectOnUp, customSkin, "Select On Up");
                            MUIPEditorHandler.DrawPropertyPlain(selectOnDown, customSkin, "Select On Down");
                            MUIPEditorHandler.DrawPropertyPlain(selectOnLeft, customSkin, "Select On Left");
                            MUIPEditorHandler.DrawPropertyPlain(selectOnRight, customSkin, "Select On Right");
                            EditorGUI.indentLevel = 0;
                        }

                        GUILayout.EndVertical();
                    }

                    GUILayout.EndVertical();

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);

                    enableButtonSounds.boolValue = MUIPEditorHandler.DrawTogglePlain(enableButtonSounds.boolValue, customSkin, "Enable Button Sounds");

                    GUILayout.Space(4);

                    if (enableButtonSounds.boolValue == true)
                    {
                        MUIPEditorHandler.DrawProperty(soundSource, customSkin, "Sound Source");
                        if (useHoverSound.boolValue == true) { MUIPEditorHandler.DrawProperty(hoverSound, customSkin, "Hover Sound"); }
                        if (useClickSound.boolValue == true) { MUIPEditorHandler.DrawProperty(clickSound, customSkin, "Click Sound"); }

                        useHoverSound.boolValue = MUIPEditorHandler.DrawToggle(useHoverSound.boolValue, customSkin, "Enable Hover Sound");
                        useClickSound.boolValue = MUIPEditorHandler.DrawToggle(useClickSound.boolValue, customSkin, "Enable Click Sound");

                        if (buttonTarget.soundSource == null) { EditorGUILayout.HelpBox("'Sound Source' is missing.", MessageType.Warning); }
                    }

                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 10);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-2);

                    useRipple.boolValue = MUIPEditorHandler.DrawTogglePlain(useRipple.boolValue, customSkin, "Use Ripple");

                    GUILayout.Space(4);

                    if (useRipple.boolValue == true)
                    {
                        renderOnTop.boolValue = MUIPEditorHandler.DrawToggle(renderOnTop.boolValue, customSkin, "Render On Top");
                        centered.boolValue = MUIPEditorHandler.DrawToggle(centered.boolValue, customSkin, "Centered");
                        MUIPEditorHandler.DrawProperty(rippleUpdateMode, customSkin, "Update Mode");
                        MUIPEditorHandler.DrawProperty(targetCanvas, customSkin, "Target Canvas");
                        MUIPEditorHandler.DrawProperty(rippleShape, customSkin, "Shape");
                        MUIPEditorHandler.DrawProperty(speed, customSkin, "Speed");
                        MUIPEditorHandler.DrawProperty(maxSize, customSkin, "Max Size");
                        MUIPEditorHandler.DrawProperty(startColor, customSkin, "Start Color");
                        MUIPEditorHandler.DrawProperty(transitionColor, customSkin, "Transition Color");
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
                                catch { Debug.LogError("<b>[Horizontal Selector]</b> Failed to delete UI Manager connection.", this); }
                            }
                        }
                    }

                    else if (tempUIM == null)
                    {
                        if (buttonTarget.isPreset == true) { MUIPEditorHandler.DrawUIManagerPresetHeader(); }
                        else
                        {
                            MUIPEditorHandler.DrawUIManagerDisconnectedHeader();

                            if (GUILayout.Button("Restore UI Manager", customSkin.button))
                            {
                                UIManagerButton uimb = buttonTarget.gameObject.AddComponent<UIManagerButton>();

                                try
                                {
                                    //
                                }

                                catch { DestroyImmediate(uimb); Debug.LogError("<b>[Modern UI Pack]</b> Cannot restore the UI Manager connection."); }
                            }
                        }
                    }

                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif