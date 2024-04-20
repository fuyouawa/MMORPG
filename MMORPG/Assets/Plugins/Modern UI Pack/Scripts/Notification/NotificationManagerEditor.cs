#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(NotificationManager))]
    public class NotificationManagerEditor : Editor
    {
        private GUISkin customSkin;
        private NotificationManager ntfTarget;
        private UIManagerNotification tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            ntfTarget = (NotificationManager)target;

            try { tempUIM = ntfTarget.GetComponent<UIManagerNotification>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Notification Top Header");

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

            var icon = serializedObject.FindProperty("icon");
            var title = serializedObject.FindProperty("title");
            var description = serializedObject.FindProperty("description");
            var notificationAnimator = serializedObject.FindProperty("notificationAnimator");
            var iconObj = serializedObject.FindProperty("iconObj");
            var titleObj = serializedObject.FindProperty("titleObj");
            var descriptionObj = serializedObject.FindProperty("descriptionObj");
            var enableTimer = serializedObject.FindProperty("enableTimer");
            var timer = serializedObject.FindProperty("timer");
            var useCustomContent = serializedObject.FindProperty("useCustomContent");
            var closeOnClick = serializedObject.FindProperty("closeOnClick");
            var useStacking = serializedObject.FindProperty("useStacking");
            var closeBehaviour = serializedObject.FindProperty("closeBehaviour");
            var startBehaviour = serializedObject.FindProperty("startBehaviour");
            var onOpen = serializedObject.FindProperty("onOpen");
            var onClose = serializedObject.FindProperty("onClose");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    MUIPEditorHandler.DrawProperty(icon, customSkin, "Icon");

                    if (ntfTarget.iconObj != null)
                        ntfTarget.iconObj.sprite = ntfTarget.icon;

                    else
                    {
                        if (ntfTarget.iconObj == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Icon Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    MUIPEditorHandler.DrawProperty(title, customSkin, "Title");

                    if (ntfTarget.titleObj != null)
                        ntfTarget.titleObj.text = title.stringValue;

                    else
                    {
                        if (ntfTarget.titleObj == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Title Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    GUILayout.BeginHorizontal(EditorStyles.helpBox);

                    EditorGUILayout.LabelField(new GUIContent("Description"), customSkin.FindStyle("Text"), GUILayout.Width(-3));
                    EditorGUILayout.PropertyField(description, new GUIContent(""), GUILayout.Height(50));

                    GUILayout.EndHorizontal();

                    if (ntfTarget.descriptionObj != null)
                        ntfTarget.descriptionObj.text = description.stringValue;

                    else
                    {
                        if (ntfTarget.descriptionObj == null)
                        {
                            GUILayout.BeginHorizontal();
                            EditorGUILayout.HelpBox("'Description Object' is not assigned. Go to Resources tab and assign the correct variable.", MessageType.Error);
                            GUILayout.EndHorizontal();
                        }
                    }

                    if (ntfTarget.GetComponent<CanvasGroup>().alpha == 0)
                    {
                        if (GUILayout.Button("Set Visible", customSkin.button))
                        {
                            ntfTarget.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    else
                    {
                        if (GUILayout.Button("Set Invisible", customSkin.button))
                        {
                            ntfTarget.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    MUIPEditorHandler.DrawHeader(customSkin, "Events Header", 10);
                    EditorGUILayout.PropertyField(onOpen, new GUIContent("On Open"), true);
                    EditorGUILayout.PropertyField(onClose, new GUIContent("On Close"), true);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(notificationAnimator, customSkin, "Animator");
                    MUIPEditorHandler.DrawProperty(iconObj, customSkin, "Icon Object");
                    MUIPEditorHandler.DrawProperty(titleObj, customSkin, "Title Object");
                    MUIPEditorHandler.DrawProperty(descriptionObj, customSkin, "Description Object");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    MUIPEditorHandler.DrawProperty(startBehaviour, customSkin, "Start Behaviour");
                    MUIPEditorHandler.DrawProperty(closeBehaviour, customSkin, "Close Behaviour");
                    useCustomContent.boolValue = MUIPEditorHandler.DrawToggle(useCustomContent.boolValue, customSkin, "Use Custom Content");
                    closeOnClick.boolValue = MUIPEditorHandler.DrawToggle(closeOnClick.boolValue, customSkin, "Close On Click");
                    useStacking.boolValue = MUIPEditorHandler.DrawToggle(useStacking.boolValue, customSkin, "Use Stacking");
                    enableTimer.boolValue = MUIPEditorHandler.DrawToggle(enableTimer.boolValue, customSkin, "Enable Timer");

                    if (enableTimer.boolValue == true)
                        MUIPEditorHandler.DrawProperty(timer, customSkin, "Timer");

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
                                catch { Debug.LogError("<b>[Notification Manager]</b> Failed to delete UI Manager connection.", this); }
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