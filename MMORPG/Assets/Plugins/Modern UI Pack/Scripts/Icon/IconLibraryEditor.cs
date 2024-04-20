#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(IconLibrary))]
    public class IconLibraryEditor : Editor
    {
        private GUISkin customSkin;

        void OnEnable()
        {
            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            // Settings
            var alwaysUpdate = serializedObject.FindProperty("alwaysUpdate");
            var optimizeUpdates = serializedObject.FindProperty("optimizeUpdates");

            MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 8);
            alwaysUpdate.boolValue = MUIPEditorHandler.DrawToggle(alwaysUpdate.boolValue, customSkin, "Always Update");
            optimizeUpdates.boolValue = MUIPEditorHandler.DrawToggle(optimizeUpdates.boolValue, customSkin, "Optimize Update");

            // Content
            var icons = serializedObject.FindProperty("icons");

            MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 8);
            GUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUI.indentLevel = 1;
            EditorGUILayout.PropertyField(icons, new GUIContent("Icon List"), true);
            EditorGUI.indentLevel = 0;

            if (GUILayout.Button("+  Add a new icon", customSkin.button))
                icons.arraySize += 1;

            GUILayout.EndVertical();

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif