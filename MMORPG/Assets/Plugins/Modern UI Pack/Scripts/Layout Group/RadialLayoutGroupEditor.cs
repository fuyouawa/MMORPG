#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(RadialLayoutGroup))]
    public class RadialLayoutGroupEditor : Editor
    {
        private GUISkin customSkin;
        private RadialLayoutGroup rlgTarget;
        private int currentTab;

        private SerializedProperty layoutDir;
        private SerializedProperty radiusStart;
        private SerializedProperty radiusRange;
        private SerializedProperty angleStart;
        private SerializedProperty angleCenter;
        private SerializedProperty angleRange;
        private SerializedProperty childRotate;

        void OnEnable()
        {
            if (this.target == null)
                return;

            this.rlgTarget = this.target as RadialLayoutGroup;

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }

            var serObj = this.serializedObject;
            this.layoutDir = serObj.FindProperty("refLayoutDir");
            this.radiusStart = serObj.FindProperty("refRadiusStart");
            this.radiusRange = serObj.FindProperty("refRadiusRange");
            this.angleStart = serObj.FindProperty("refAngleStart");
            this.angleCenter = serObj.FindProperty("refAngleCenter");
            this.angleRange = serObj.FindProperty("refAngleRange");
            this.childRotate = serObj.FindProperty("refChildRotate");
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "RLG Top Header");

            GUIContent[] toolbarTabs = new GUIContent[1];
            toolbarTabs[0] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 0;

            GUILayout.EndHorizontal();
            serializedObject.Update();

            MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);

            GUILayout.BeginVertical(EditorStyles.helpBox);
            MUIPEditorHandler.DrawPropertyPlain(layoutDir, customSkin, "Layout Direction");
            EditorGUI.indentLevel = 1;

            if (rlgTarget.layoutDir != RadialLayoutGroup.Direction.Bidirectional)
                EditorGUILayout.PropertyField(angleStart, new GUIContent("Angle Start"));
            else
                EditorGUILayout.PropertyField(angleCenter, new GUIContent("Angle Center"));

            EditorGUILayout.PropertyField(angleRange, new GUIContent("Angle Range"));
            EditorGUILayout.PropertyField(radiusStart, new GUIContent("Radius Start"));
            EditorGUILayout.PropertyField(radiusRange, new GUIContent("Radius Range"));

            EditorGUI.indentLevel = 0;
            GUILayout.EndVertical();

            childRotate.boolValue = MUIPEditorHandler.DrawToggle(childRotate.boolValue, customSkin, "Rotate Child");

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }
    }
}
#endif