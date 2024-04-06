#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.Presets;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(UIManager))]
    [System.Serializable]
    public class UIManagerEditor : Editor
    {
        GUISkin customSkin;
        protected static string buildID = "B16-20231120";
        protected static float foldoutItemSpace = 2;
        protected static float foldoutTopSpace = 5;
        protected static float foldoutBottomSpace = 2;

        protected static bool showAnimatedIcon = false;
        protected static bool showButton = false;
        protected static bool showContext = false;
        protected static bool showDropdown = false;
        protected static bool showHorSelector = false;
        protected static bool showInputField = false;
        protected static bool showModalWindow = false;
        protected static bool showNotification = false;
        protected static bool showProgressBar = false;
        protected static bool showScrollbar = false;
        protected static bool showSlider = false;
        protected static bool showSwitch = false;
        protected static bool showToggle = false;
        protected static bool showTooltip = false;
        protected static bool showCustomObjects = false;

        private void OnEnable() 
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            if (customSkin == null)
            {
                EditorGUILayout.HelpBox("Editor variables are missing. You can manually fix this by deleting " +
                    "Modern UI Pack > Resources folder and then re-import the package. \n\nIf you're still seeing this " +
                    "dialog even after the re-import, contact me with this ID: " + buildID, MessageType.Error);
              
                if (GUILayout.Button("Contact")) { Email(); }
                return;
            }

            // Foldout style
            GUIStyle foldoutStyle = customSkin.FindStyle("UIM Foldout");

            // UIM Header
            MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 8);
            GUILayout.BeginVertical(EditorStyles.helpBox);

            // Animated Icon
            var animatedIconColor = serializedObject.FindProperty("animatedIconColor");
          
            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showAnimatedIcon = EditorGUILayout.Foldout(showAnimatedIcon, "Animated Icon", true, foldoutStyle);
            showAnimatedIcon = GUILayout.Toggle(showAnimatedIcon, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showAnimatedIcon)
            {
                MUIPEditorHandler.DrawProperty(animatedIconColor, customSkin, "Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);

            #region Button
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var buttonFont = serializedObject.FindProperty("buttonFont");
            var buttonNormalColor = serializedObject.FindProperty("buttonNormalColor");
            var buttonAccentColor = serializedObject.FindProperty("buttonAccentColor");
            var buttonDisabledAlpha = serializedObject.FindProperty("buttonDisabledAlpha");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showButton = EditorGUILayout.Foldout(showButton, "Button", true, foldoutStyle);
            showButton = GUILayout.Toggle(showButton, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showButton)
            {
                MUIPEditorHandler.DrawProperty(buttonFont, customSkin, "Font");
                MUIPEditorHandler.DrawProperty(buttonNormalColor, customSkin, "Normal Color");
                MUIPEditorHandler.DrawProperty(buttonAccentColor, customSkin, "Accent Color");
                MUIPEditorHandler.DrawProperty(buttonDisabledAlpha, customSkin, "Disabled Alpha");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Context Menu
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var contextBackgroundColor = serializedObject.FindProperty("contextBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showContext = EditorGUILayout.Foldout(showContext, "Context Menu", true, foldoutStyle);
            showContext = GUILayout.Toggle(showContext, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showContext)
            {
                MUIPEditorHandler.DrawProperty(contextBackgroundColor, customSkin, "Background Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Dropdown
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var dropdownFont = serializedObject.FindProperty("dropdownFont");
            var dropdownItemFont = serializedObject.FindProperty("dropdownItemFont");
            var dropdownPrimaryColor = serializedObject.FindProperty("dropdownPrimaryColor");
            var dropdownBackgroundColor = serializedObject.FindProperty("dropdownBackgroundColor");
            var dropdownItemBackgroundColor = serializedObject.FindProperty("dropdownItemBackgroundColor");
            var dropdownItemPrimaryColor = serializedObject.FindProperty("dropdownItemPrimaryColor");
            var dropdownContentBackgroundColor = serializedObject.FindProperty("dropdownContentBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showDropdown = EditorGUILayout.Foldout(showDropdown, "Dropdown", true, foldoutStyle);
            showDropdown = GUILayout.Toggle(showDropdown, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showDropdown)
            {
                MUIPEditorHandler.DrawProperty(dropdownFont, customSkin, "Font");
                MUIPEditorHandler.DrawProperty(dropdownItemFont, customSkin, "Item Font");
                MUIPEditorHandler.DrawProperty(dropdownPrimaryColor, customSkin, "Primary");
                MUIPEditorHandler.DrawProperty(dropdownBackgroundColor, customSkin, "Background");
                MUIPEditorHandler.DrawProperty(dropdownContentBackgroundColor, customSkin, "Content Background");
                MUIPEditorHandler.DrawProperty(dropdownItemBackgroundColor, customSkin, "Item Background");
                MUIPEditorHandler.DrawProperty(dropdownItemPrimaryColor, customSkin, "Item Primary");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Horizontal Selector
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var selectorFont = serializedObject.FindProperty("selectorFont");
            var selectorColor = serializedObject.FindProperty("selectorColor");
            var selectorHighlightedColor = serializedObject.FindProperty("selectorHighlightedColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showHorSelector = EditorGUILayout.Foldout(showHorSelector, "Horizontal Selector", true, foldoutStyle);
            showHorSelector = GUILayout.Toggle(showHorSelector, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showHorSelector)
            {
                MUIPEditorHandler.DrawProperty(selectorFont, customSkin, "Font");
                MUIPEditorHandler.DrawProperty(selectorColor, customSkin, "Color");
                MUIPEditorHandler.DrawProperty(selectorHighlightedColor, customSkin, "Highlighted Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Input Field
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var inputFieldFont = serializedObject.FindProperty("inputFieldFont");
            var inputFieldColor = serializedObject.FindProperty("inputFieldColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showInputField = EditorGUILayout.Foldout(showInputField, "Input Field", true, foldoutStyle);
            showInputField = GUILayout.Toggle(showInputField, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showInputField)
            {
                MUIPEditorHandler.DrawProperty(inputFieldFont, customSkin, "Font");
                MUIPEditorHandler.DrawProperty(inputFieldColor, customSkin, "Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Modal Window
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var modalWindowTitleFont = serializedObject.FindProperty("modalWindowTitleFont");
            var modalWindowContentFont = serializedObject.FindProperty("modalWindowContentFont");
            var modalWindowTitleColor = serializedObject.FindProperty("modalWindowTitleColor");
            var modalWindowDescriptionColor = serializedObject.FindProperty("modalWindowDescriptionColor");
            var modalWindowIconColor = serializedObject.FindProperty("modalWindowIconColor");
            var modalWindowBackgroundColor = serializedObject.FindProperty("modalWindowBackgroundColor");
            var modalWindowContentPanelColor = serializedObject.FindProperty("modalWindowContentPanelColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showModalWindow = EditorGUILayout.Foldout(showModalWindow, "Modal Window", true, foldoutStyle);
            showModalWindow = GUILayout.Toggle(showModalWindow, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showModalWindow)
            {
                MUIPEditorHandler.DrawProperty(modalWindowTitleFont, customSkin, "Title Font");
                MUIPEditorHandler.DrawProperty(modalWindowContentFont, customSkin, "Content Font");
                MUIPEditorHandler.DrawProperty(modalWindowTitleColor, customSkin, "Title Color");
                MUIPEditorHandler.DrawProperty(modalWindowDescriptionColor, customSkin, "Description Color");
                MUIPEditorHandler.DrawProperty(modalWindowIconColor, customSkin, "Icon Color");
                MUIPEditorHandler.DrawProperty(modalWindowBackgroundColor, customSkin, "Background Color");
                MUIPEditorHandler.DrawProperty(modalWindowContentPanelColor, customSkin, "Content Panel Color");
                EditorGUILayout.HelpBox("These values will only affect 'Style 1 - Standard' window.", MessageType.Info);
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Notification
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var notificationTitleFont = serializedObject.FindProperty("notificationTitleFont");
            var notificationTitleFontSize = serializedObject.FindProperty("notificationTitleFontSize");
            var notificationDescriptionFont = serializedObject.FindProperty("notificationDescriptionFont");
            var notificationDescriptionFontSize = serializedObject.FindProperty("notificationDescriptionFontSize");
            var notificationBackgroundColor = serializedObject.FindProperty("notificationBackgroundColor");
            var notificationTitleColor = serializedObject.FindProperty("notificationTitleColor");
            var notificationDescriptionColor = serializedObject.FindProperty("notificationDescriptionColor");
            var notificationIconColor = serializedObject.FindProperty("notificationIconColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showNotification = EditorGUILayout.Foldout(showNotification, "Notification", true, foldoutStyle);
            showNotification = GUILayout.Toggle(showNotification, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showNotification)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Title Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationTitleFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(notificationTitleFont, new GUIContent(""));
                GUILayout.EndHorizontal();
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Description Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(notificationDescriptionFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(notificationDescriptionFont, new GUIContent(""));
                GUILayout.EndHorizontal();
                MUIPEditorHandler.DrawProperty(notificationBackgroundColor, customSkin, "Background Color");
                MUIPEditorHandler.DrawProperty(notificationTitleColor, customSkin, "Title Color");
                MUIPEditorHandler.DrawProperty(notificationDescriptionColor, customSkin, "Description Color");
                MUIPEditorHandler.DrawProperty(notificationIconColor, customSkin, "Icon Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Progress Bar
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var progressBarLabelFont = serializedObject.FindProperty("progressBarLabelFont");
            var progressBarColor = serializedObject.FindProperty("progressBarColor");
            var progressBarBackgroundColor = serializedObject.FindProperty("progressBarBackgroundColor");
            var progressBarLoopBackgroundColor = serializedObject.FindProperty("progressBarLoopBackgroundColor");
            var progressBarLabelColor = serializedObject.FindProperty("progressBarLabelColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showProgressBar = EditorGUILayout.Foldout(showProgressBar, "Progress Bar", true, foldoutStyle);
            showProgressBar = GUILayout.Toggle(showProgressBar, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showProgressBar)
            {
                MUIPEditorHandler.DrawProperty(progressBarLabelFont, customSkin, "Label Font");
                MUIPEditorHandler.DrawProperty(progressBarColor, customSkin, "Color");
                MUIPEditorHandler.DrawProperty(progressBarLabelColor, customSkin, "Label Color");
                MUIPEditorHandler.DrawProperty(progressBarBackgroundColor, customSkin, "Background Color");
                MUIPEditorHandler.DrawProperty(progressBarLoopBackgroundColor, customSkin, "Loop BG Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Scrollbar
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var scrollbarColor = serializedObject.FindProperty("scrollbarColor");
            var scrollbarBackgroundColor = serializedObject.FindProperty("scrollbarBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showScrollbar = EditorGUILayout.Foldout(showScrollbar, "Scrollbar", true, foldoutStyle);
            showScrollbar = GUILayout.Toggle(showScrollbar, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showScrollbar)
            {
                MUIPEditorHandler.DrawProperty(scrollbarColor, customSkin, "Bar Color");
                MUIPEditorHandler.DrawProperty(scrollbarBackgroundColor, customSkin, "Background Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Slider
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var sliderThemeType = serializedObject.FindProperty("sliderThemeType");
            var sliderLabelFont = serializedObject.FindProperty("sliderLabelFont");
            var sliderColor = serializedObject.FindProperty("sliderColor");
            var sliderLabelColor = serializedObject.FindProperty("sliderLabelColor");
            var sliderPopupLabelColor = serializedObject.FindProperty("sliderPopupLabelColor");
            var sliderHandleColor = serializedObject.FindProperty("sliderHandleColor");
            var sliderBackgroundColor = serializedObject.FindProperty("sliderBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showSlider = EditorGUILayout.Foldout(showSlider, "Slider", true, foldoutStyle);
            showSlider = GUILayout.Toggle(showSlider, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showSlider && sliderThemeType.enumValueIndex == 0)
            {
                MUIPEditorHandler.DrawProperty(sliderThemeType, customSkin, "Theme Type");
                MUIPEditorHandler.DrawProperty(sliderLabelFont, customSkin, "Label Font");
                MUIPEditorHandler.DrawProperty(sliderColor, customSkin, "Primary Color");
                MUIPEditorHandler.DrawProperty(sliderBackgroundColor, customSkin, "Secondary Color");
                MUIPEditorHandler.DrawProperty(sliderLabelColor, customSkin, "Label Popup Color");
            }

            if (showSlider && sliderThemeType.enumValueIndex == 1)
            {
                MUIPEditorHandler.DrawProperty(sliderThemeType, customSkin, "Theme Type");
                MUIPEditorHandler.DrawProperty(sliderLabelFont, customSkin, "Label Font");
                MUIPEditorHandler.DrawProperty(sliderColor, customSkin, "Color");
                MUIPEditorHandler.DrawProperty(sliderLabelColor, customSkin, "Label Color");
                MUIPEditorHandler.DrawProperty(sliderPopupLabelColor, customSkin, "Label Popup Color");
                MUIPEditorHandler.DrawProperty(sliderHandleColor, customSkin, "Handle Color");
                MUIPEditorHandler.DrawProperty(sliderBackgroundColor, customSkin, "Background Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Switch
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var switchBorderColor = serializedObject.FindProperty("switchBorderColor");
            var switchBackgroundColor = serializedObject.FindProperty("switchBackgroundColor");
            var switchHandleOnColor = serializedObject.FindProperty("switchHandleOnColor");
            var switchHandleOffColor = serializedObject.FindProperty("switchHandleOffColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showSwitch = EditorGUILayout.Foldout(showSwitch, "Switch", true, foldoutStyle);
            showSwitch = GUILayout.Toggle(showSwitch, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showSwitch)
            {
                MUIPEditorHandler.DrawProperty(switchBorderColor, customSkin, "Border Color");
                MUIPEditorHandler.DrawProperty(switchBackgroundColor, customSkin, "Background Color");
                MUIPEditorHandler.DrawProperty(switchHandleOnColor, customSkin, "Handle On Color");
                MUIPEditorHandler.DrawProperty(switchHandleOffColor, customSkin, "Handle Off Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Toggle
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var toggleFont = serializedObject.FindProperty("toggleFont");
            var toggleTextColor = serializedObject.FindProperty("toggleTextColor");
            var toggleBorderColor = serializedObject.FindProperty("toggleBorderColor");
            var toggleBackgroundColor = serializedObject.FindProperty("toggleBackgroundColor");
            var toggleCheckColor = serializedObject.FindProperty("toggleCheckColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showToggle = EditorGUILayout.Foldout(showToggle, "Toggle", true, foldoutStyle);
            showToggle = GUILayout.Toggle(showToggle, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showToggle)
            {
                MUIPEditorHandler.DrawProperty(toggleFont, customSkin, "Font");
                MUIPEditorHandler.DrawProperty(toggleTextColor, customSkin, "Text Color");
                MUIPEditorHandler.DrawProperty(toggleBorderColor, customSkin, "Border Color");
                MUIPEditorHandler.DrawProperty(toggleBackgroundColor, customSkin, "Background Color");
                MUIPEditorHandler.DrawProperty(toggleCheckColor, customSkin, "Check Color");
            }

            GUILayout.EndVertical();
            GUILayout.Space(foldoutItemSpace);
            #endregion

            #region Tooltip
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var tooltipFont = serializedObject.FindProperty("tooltipFont");
            var tooltipFontSize = serializedObject.FindProperty("tooltipFontSize");
            var tooltipTextColor = serializedObject.FindProperty("tooltipTextColor");
            var tooltipBackgroundColor = serializedObject.FindProperty("tooltipBackgroundColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showTooltip = EditorGUILayout.Foldout(showTooltip, "Tooltip", true, foldoutStyle);
            showTooltip = GUILayout.Toggle(showTooltip, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showTooltip)
            {
                GUILayout.BeginHorizontal(EditorStyles.helpBox);
                EditorGUILayout.LabelField(new GUIContent("Font"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                EditorGUILayout.PropertyField(tooltipFontSize, new GUIContent(""), GUILayout.Width(40));
                EditorGUILayout.PropertyField(tooltipFont, new GUIContent(""));
                GUILayout.EndHorizontal();
                MUIPEditorHandler.DrawProperty(tooltipTextColor, customSkin, "Text Color");
                MUIPEditorHandler.DrawProperty(tooltipBackgroundColor, customSkin, "Background Color");
            }

            GUILayout.EndVertical();
            #endregion

            #region Custom Objects
            GUILayout.BeginVertical(EditorStyles.helpBox);

            var customObjPrimaryFont = serializedObject.FindProperty("customObjPrimaryFont");
            var customObjSecondaryFont = serializedObject.FindProperty("customObjSecondaryFont");
            var customObjPrimaryColor = serializedObject.FindProperty("customObjPrimaryColor");
            var customObjSecondaryColor = serializedObject.FindProperty("customObjSecondaryColor");

            GUILayout.Space(foldoutTopSpace);
            GUILayout.BeginHorizontal();
            showCustomObjects = EditorGUILayout.Foldout(showCustomObjects, "Custom Objects", true, foldoutStyle);
            showCustomObjects = GUILayout.Toggle(showCustomObjects, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(foldoutBottomSpace);

            if (showCustomObjects)
            {
                MUIPEditorHandler.DrawProperty(customObjPrimaryFont, customSkin, "Primary Font");
                MUIPEditorHandler.DrawProperty(customObjSecondaryFont, customSkin, "Secondary Font");
                MUIPEditorHandler.DrawProperty(customObjPrimaryColor, customSkin, "Primary Color");
                MUIPEditorHandler.DrawProperty(customObjSecondaryColor, customSkin, "Secondary Color");
            }

            GUILayout.EndVertical();
            #endregion

            #region Settings
            MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 14);

            var enableDynamicUpdate = serializedObject.FindProperty("enableDynamicUpdate");
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(-2);
            GUILayout.BeginHorizontal();
            enableDynamicUpdate.boolValue = GUILayout.Toggle(enableDynamicUpdate.boolValue, new GUIContent("Enable Dynamic Update"), customSkin.FindStyle("Toggle"));
            enableDynamicUpdate.boolValue = GUILayout.Toggle(enableDynamicUpdate.boolValue, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
            GUILayout.EndHorizontal();
            GUILayout.Space(4);

            if (enableDynamicUpdate.boolValue == true)
            {
                EditorGUILayout.HelpBox("When this option is enabled, all objects connected to this manager will be dynamically updated synchronously. " +
                    "Basically; consumes more resources, but allows dynamic changes at runtime/editor.", MessageType.Info);
            }

            else
            {
                EditorGUILayout.HelpBox("When this option is disabled, all objects connected to this manager will be updated only once on awake. " +
                    "Basically; has better performance, but it's static.", MessageType.Info);
            }

            GUILayout.EndVertical();

            var editorHints = serializedObject.FindProperty("editorHints");
            GUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayout.Space(-3);
            editorHints.boolValue = MUIPEditorHandler.DrawTogglePlain(editorHints.boolValue, customSkin, "UI Manager Hints");
            GUILayout.Space(3);

            if (editorHints.boolValue == true)
            {
                EditorGUILayout.HelpBox("These values are universal and affect all objects containing 'UI Manager' component.", MessageType.Info);
                EditorGUILayout.HelpBox("If want to assign unique values, remove 'UI Manager' component from the object ", MessageType.Info);
				EditorGUILayout.HelpBox("You can press 'Control/Command + Shift + M' to open the manager quickly.", MessageType.Info);
            }

            GUILayout.EndVertical();
            #endregion

            serializedObject.ApplyModifiedProperties();
            if (Application.isPlaying == false) { this.Repaint(); }

            GUILayout.Space(12);
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Reset to defaults", customSkin.button))
                ResetToDefaults();

            GUILayout.EndHorizontal();

            #region Support
            MUIPEditorHandler.DrawHeader(customSkin, "Support Header", 14);
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Need help? Contact me via:", customSkin.FindStyle("Text"));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("Discord", customSkin.button)) { Discord(); }
            if (GUILayout.Button("Twitter", customSkin.button)) { Twitter(); }

            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();

            if (GUILayout.Button("E-mail", customSkin.button)) { Email(); }

            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.Space(6);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.Label("ID: " + buildID);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.Space(6);
            #endregion
        }

        void Discord() { Application.OpenURL("https://discord.gg/VXpHyUt"); }
        void Email() { Application.OpenURL("https://www.michsky.com/contact/"); }
        void Twitter() { Application.OpenURL("https://www.twitter.com/michskyHQ"); }

        void ResetToDefaults()
        {
            if (EditorUtility.DisplayDialog("Reset to defaults", "Are you sure you want to reset UI Manager values to default?", "Yes", "Cancel"))
            {
                try
                {
                    Preset defaultPreset = Resources.Load<Preset>("UI Manager Presets/Default");
                    defaultPreset.ApplyTo(Resources.Load("MUIP Manager"));
                    Selection.activeObject = null;
                    Debug.Log("<b>[UI Manager]</b> Resetting successful.");
                }

                catch { Debug.LogWarning("<b>[UI Manager]</b> Resetting failed. Default preset seems to be missing"); }
            }    
        }
    }
}
#endif