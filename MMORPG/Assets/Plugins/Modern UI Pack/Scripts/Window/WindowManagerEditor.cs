#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(WindowManager))]
    public class WindowManagerEditor : Editor
    {
        private GUISkin customSkin;
        private WindowManager wmTarget;
        private UIManagerWindowManager tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            wmTarget = (WindowManager)target;

            try { tempUIM = wmTarget.GetComponent<UIManagerWindowManager>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "WM Top Header");

            GUIContent[] toolbarTabs = new GUIContent[2];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 1;

            GUILayout.EndHorizontal();

            var windows = serializedObject.FindProperty("windows");
            var currentWindowIndex = serializedObject.FindProperty("currentWindowIndex");
            var cullWindows = serializedObject.FindProperty("cullWindows");
            var onWindowChange = serializedObject.FindProperty("onWindowChange");
            var initializeButtons = serializedObject.FindProperty("initializeButtons");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);

                    if (wmTarget.windows.Count != 0)
                    {
                        if (Application.isPlaying == true) { GUI.enabled = false; }
                        GUILayout.BeginVertical(EditorStyles.helpBox);

                        EditorGUILayout.LabelField(new GUIContent("Selected Window:"), customSkin.FindStyle("Text"), GUILayout.Width(120));
                        currentWindowIndex.intValue = EditorGUILayout.IntSlider(currentWindowIndex.intValue, 0, wmTarget.windows.Count - 1);

                        GUILayout.Space(2);
                        EditorGUILayout.LabelField(new GUIContent(wmTarget.windows[currentWindowIndex.intValue].windowName), customSkin.FindStyle("Text"));

                        if (Application.isPlaying == false && wmTarget.windows[currentWindowIndex.intValue].windowObject != null)
                        {
                            for (int i = 0; i < wmTarget.windows.Count; i++)
                            {
                                if (i == currentWindowIndex.intValue) 
                                {
                                    var tempCG = wmTarget.windows[currentWindowIndex.intValue].windowObject.GetComponent<CanvasGroup>();
                                    if (tempCG != null) { tempCG.alpha = 1; }
                                }

                                else if (wmTarget.windows[i].windowObject != null)
                                {
                                    var tempCG = wmTarget.windows[i].windowObject.GetComponent<CanvasGroup>();
                                    if (tempCG != null) { tempCG.alpha = 0; }
                                }
                            }
                        }

                        GUI.enabled = true;
                        GUILayout.EndVertical();
                    }

                    else { EditorGUILayout.HelpBox("Window List is empty. Create a new item to see more options.", MessageType.Info); }

                    GUILayout.BeginVertical();
                    EditorGUI.indentLevel = 1;

                    EditorGUILayout.PropertyField(windows, new GUIContent("Window Items"), true);

                    if (GUILayout.Button("+  Add a new window", customSkin.button))
                        wmTarget.AddNewItem();

                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onWindowChange, new GUIContent("On Window Change"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    cullWindows.boolValue = MUIPEditorHandler.DrawToggle(cullWindows.boolValue, customSkin, "Cull Transparent Windows");
                    initializeButtons.boolValue = MUIPEditorHandler.DrawToggle(initializeButtons.boolValue, customSkin, "Initialize Buttons");

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
                                catch { Debug.LogError("<b>[Window Manager]</b> Failed to delete UI Manager connection.", this); }
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