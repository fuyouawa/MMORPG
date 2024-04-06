#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(UIGradient))]
    public class UIGradientEditor : Editor
    {
        private GUISkin customSkin;
        private int currentTab;

        private void OnEnable()
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "Gradient Top Header");

            GUIContent[] toolbarTabs = new GUIContent[1];
            toolbarTabs[0] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 0;

            GUILayout.EndHorizontal();

            var _effectGradient = serializedObject.FindProperty("_effectGradient");
            var _gradientType = serializedObject.FindProperty("_gradientType");
            var _offset = serializedObject.FindProperty("_offset");
            var _zoom = serializedObject.FindProperty("_zoom");
            var _modifyVertices = serializedObject.FindProperty("_modifyVertices");

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    MUIPEditorHandler.DrawPropertyCW(_effectGradient, customSkin, "Gradient", 100);
                    MUIPEditorHandler.DrawPropertyCW(_gradientType, customSkin, "Type", 100);
                    MUIPEditorHandler.DrawPropertyCW(_offset, customSkin, "Offset", 100);
                    MUIPEditorHandler.DrawPropertyCW(_zoom, customSkin, "Zoom", 100);
                    _modifyVertices.boolValue = MUIPEditorHandler.DrawToggle(_modifyVertices.boolValue, customSkin, "Complex Gradient");
                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif