#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(SwitchManager))]
    public class SwitchManagerEditor : Editor
    {
        private GUISkin customSkin;
        private SwitchManager switchTarget;
        private UIManagerSwitch tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            switchTarget = (SwitchManager)target;

            try { tempUIM = switchTarget.GetComponent<UIManagerSwitch>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Switch Top Header");

            GUIContent[] toolbarTabs = new GUIContent[2];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Settings");
    
            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 1;

            GUILayout.EndHorizontal();

            var onValueChanged = serializedObject.FindProperty("onValueChanged");
            var OnEvents = serializedObject.FindProperty("OnEvents");
            var OffEvents = serializedObject.FindProperty("OffEvents");
            var saveValue = serializedObject.FindProperty("saveValue");
            var switchTag = serializedObject.FindProperty("switchTag");
            var invokeAtStart = serializedObject.FindProperty("invokeAtStart");
            var isOn = serializedObject.FindProperty("isOn");
            var enableSwitchSounds = serializedObject.FindProperty("enableSwitchSounds");
            var useHoverSound = serializedObject.FindProperty("useHoverSound");
            var useClickSound = serializedObject.FindProperty("useClickSound");
            var soundSource = serializedObject.FindProperty("soundSource");
            var hoverSound = serializedObject.FindProperty("hoverSound");
            var clickSound = serializedObject.FindProperty("clickSound");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 6);
                    EditorGUILayout.PropertyField(onValueChanged, new GUIContent("On Value Changed"), true);
                    EditorGUILayout.PropertyField(OnEvents, new GUIContent("On Events"), true);
                    EditorGUILayout.PropertyField(OffEvents, new GUIContent("Off Events"), true);

                    if (enableSwitchSounds.boolValue == true && useHoverSound.boolValue == true)
                        MUIPEditorHandler.DrawProperty(hoverSound, customSkin, "Hover Sound");

                    if (enableSwitchSounds.boolValue == true && useClickSound.boolValue == true)
                        MUIPEditorHandler.DrawProperty(clickSound, customSkin, "Click Sound");

                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    invokeAtStart.boolValue = MUIPEditorHandler.DrawToggle(invokeAtStart.boolValue, customSkin, "Invoke At Start");
                    isOn.boolValue = MUIPEditorHandler.DrawToggle(isOn.boolValue, customSkin, "Is On");

                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    enableSwitchSounds.boolValue = MUIPEditorHandler.DrawTogglePlain(enableSwitchSounds.boolValue, customSkin, "Enable Switch Sounds");
                    GUILayout.Space(3);

                    if (enableSwitchSounds.boolValue == true)
                    {
                        MUIPEditorHandler.DrawProperty(soundSource, customSkin, "Sound Source");

                        useHoverSound.boolValue = MUIPEditorHandler.DrawToggle(useHoverSound.boolValue, customSkin, "Enable Hover Sound");
                        useClickSound.boolValue = MUIPEditorHandler.DrawToggle(useClickSound.boolValue, customSkin, "Enable Click Sound");

                        if (useHoverSound.boolValue == true) { MUIPEditorHandler.DrawProperty(hoverSound, customSkin, "Hover Sound"); }
                        if (useClickSound.boolValue == true) { MUIPEditorHandler.DrawProperty(clickSound, customSkin, "Click Sound"); }

                        if (switchTarget.soundSource == null) { EditorGUILayout.HelpBox("'Sound Source' is missing.", MessageType.Warning); }
                    }

                    GUILayout.EndVertical();
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.Space(-3);
                    saveValue.boolValue = MUIPEditorHandler.DrawTogglePlain(saveValue.boolValue, customSkin, "Save Value");
                    GUILayout.Space(3);

                    if (saveValue.boolValue == true)
                    {
                        MUIPEditorHandler.DrawPropertyPlainCW(switchTag, customSkin, "Switch Tag:", 90);
                        EditorGUILayout.HelpBox("Each switch should has its own unique tag.", MessageType.Info);
                    }

                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "UIM Header", 10);

                    if (tempUIM != null)
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