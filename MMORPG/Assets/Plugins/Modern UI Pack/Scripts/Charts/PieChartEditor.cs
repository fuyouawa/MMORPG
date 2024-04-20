#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(PieChart))]
    public class PieChartEditor : Editor
    {
        private GUISkin customSkin;
        private PieChart pieTarget;
        private UIManagerPieChart tempUIM;
        private int currentTab;

        private void OnEnable()
        {
            pieTarget = (PieChart)target;

            try { tempUIM = pieTarget.GetComponent<UIManagerPieChart>(); }
            catch { }

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "PC Top Header");

            GUIContent[] toolbarTabs = new GUIContent[2];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 1;

            GUILayout.EndHorizontal();

            var chartData = serializedObject.FindProperty("chartData");
            var borderThickness = serializedObject.FindProperty("borderThickness");
            var borderColor = serializedObject.FindProperty("borderColor");
            var enableBorderColor = serializedObject.FindProperty("enableBorderColor");
            var addValueToIndicator = serializedObject.FindProperty("addValueToIndicator");
            var indicatorParent = serializedObject.FindProperty("indicatorParent");
            var valuePrefix = serializedObject.FindProperty("valuePrefix");
            var valueSuffix = serializedObject.FindProperty("valueSuffix");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel = 1;
                  
                    EditorGUILayout.PropertyField(chartData, new GUIContent("Chart Items"));
                    chartData.isExpanded = true;

                    if (GUILayout.Button("+  Add a new item", customSkin.button))
                        pieTarget.AddNewItem();

                    EditorGUI.indentLevel = 0;
                    GUILayout.EndHorizontal();

                    if (pieTarget.gameObject.activeInHierarchy == true)
                        pieTarget.UpdateIndicators();

                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 6);
                    MUIPEditorHandler.DrawProperty(indicatorParent, customSkin, "Indicator Parent");
                    MUIPEditorHandler.DrawProperty(borderThickness, customSkin, "Border Thickness");
                    addValueToIndicator.boolValue = MUIPEditorHandler.DrawToggle(addValueToIndicator.boolValue, customSkin, "Add Value To Indicator");

                    if (addValueToIndicator.boolValue == true)
                    {
                        MUIPEditorHandler.DrawPropertyCW(valuePrefix, customSkin, "Value Prefix:", 75);
                        MUIPEditorHandler.DrawPropertyCW(valueSuffix, customSkin, "Value Suffix:", 75);
                    }

                    enableBorderColor.boolValue = MUIPEditorHandler.DrawToggle(enableBorderColor.boolValue, customSkin, "Enable Border Color (Experimental)");

                    if (enableBorderColor.boolValue == true)
                        MUIPEditorHandler.DrawProperty(borderColor, customSkin, "Border Color");

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

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif