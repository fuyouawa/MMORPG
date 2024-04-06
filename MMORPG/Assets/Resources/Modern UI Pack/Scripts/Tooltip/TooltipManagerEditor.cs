#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(TooltipManager))]
    public class TooltipManagerEditor : Editor
    {
        private GUISkin customSkin;
        private TooltipManager tooltipTarget;
        private UIManagerTooltip tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            tooltipTarget = (TooltipManager)target;

            try { tempUIM = tooltipTarget.GetComponent<UIManagerTooltip>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Tooltip Top Header");

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

            var vBorderTop = serializedObject.FindProperty("vBorderTop");
            var vBorderBottom = serializedObject.FindProperty("vBorderBottom");
            var hBorderLeft = serializedObject.FindProperty("hBorderLeft");
            var hBorderRight = serializedObject.FindProperty("hBorderRight");
            var mainCanvas = serializedObject.FindProperty("mainCanvas");
            var tooltipObject = serializedObject.FindProperty("tooltipObject");
            var tooltipContent = serializedObject.FindProperty("tooltipContent");
            var tooltipSmoothness = serializedObject.FindProperty("tooltipSmoothness");
            var dampSpeed = serializedObject.FindProperty("dampSpeed");
            var preferredWidth = serializedObject.FindProperty("preferredWidth");
            var targetCamera = serializedObject.FindProperty("targetCamera");
            var cameraSource = serializedObject.FindProperty("cameraSource");
            var transitionMode = serializedObject.FindProperty("transitionMode");
            var checkDispose = serializedObject.FindProperty("checkDispose");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    MUIPEditorHandler.DrawProperty(vBorderTop, customSkin, "Top Bound");
                    MUIPEditorHandler.DrawProperty(vBorderBottom, customSkin, "Bottom Bound");
                    MUIPEditorHandler.DrawProperty(hBorderLeft, customSkin, "Left Bound");
                    MUIPEditorHandler.DrawProperty(hBorderRight, customSkin, "Right Bound");

                    if (tooltipTarget.tooltipObject != null && tooltipTarget.tooltipObject.GetComponent<CanvasGroup>().alpha == 0)
                    {
                        if (GUILayout.Button("Make It Visible", customSkin.button))
                        {
                            tooltipTarget.tooltipObject.GetComponent<CanvasGroup>().alpha = 1;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    else
                    {
                        if (GUILayout.Button("Make It Invisible", customSkin.button))
                        {
                            tooltipTarget.tooltipObject.GetComponent<CanvasGroup>().alpha = 0;
                            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
                        }
                    }

                    EditorGUILayout.HelpBox("In order to use the tooltip system, you should add the 'Tooltip Content' component to your objects.", MessageType.Info);
                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(tooltipObject, customSkin, "Tooltip Object");
                    MUIPEditorHandler.DrawProperty(tooltipContent, customSkin, "Tooltip Content");
                    MUIPEditorHandler.DrawProperty(mainCanvas, customSkin, "Main Canvas");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    checkDispose.boolValue = MUIPEditorHandler.DrawToggle(checkDispose.boolValue, customSkin, "Check Dispose/Null");
                    MUIPEditorHandler.DrawProperty(preferredWidth, customSkin, "Preferred Width");
                    MUIPEditorHandler.DrawProperty(tooltipSmoothness, customSkin, "Smoothness");
                    MUIPEditorHandler.DrawProperty(dampSpeed, customSkin, "Damp Speed");
                    MUIPEditorHandler.DrawProperty(transitionMode, customSkin, "Transition Mode");
                    MUIPEditorHandler.DrawProperty(cameraSource, customSkin, "Camera Source");

                    if (tooltipTarget.cameraSource == TooltipManager.CameraSource.Custom)
                        MUIPEditorHandler.DrawProperty(targetCamera, customSkin, "Target Camera");

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