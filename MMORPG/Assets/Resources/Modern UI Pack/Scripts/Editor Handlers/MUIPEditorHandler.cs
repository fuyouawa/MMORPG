#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

namespace Michsky.MUIP
{
    public class MUIPEditorHandler : Editor
    {
        public static GUISkin GetDarkEditor(GUISkin tempSkin)
        {
            tempSkin = (GUISkin)Resources.Load("MUIP-EditorDark");
            if (tempSkin == null) { tempSkin = (GUISkin)Resources.Load("MUI Skin Dark"); }
            return tempSkin;
        }

        public static GUISkin GetLightEditor(GUISkin tempSkin)
        {
            tempSkin = (GUISkin)Resources.Load("MUIP-EditorLight");
            if (tempSkin == null) { tempSkin = (GUISkin)Resources.Load("MUI Skin Light"); }
            return tempSkin;
        }

        public static void DrawProperty(SerializedProperty property, GUISkin skin, string content)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(120));
            EditorGUILayout.PropertyField(property, new GUIContent(""));

            GUILayout.EndHorizontal();
        }

        public static void DrawPropertyPlain(SerializedProperty property, GUISkin skin, string content)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(120));
            EditorGUILayout.PropertyField(property, new GUIContent(""));

            GUILayout.EndHorizontal();
        }

        public static void DrawPropertyCW(SerializedProperty property, GUISkin skin, string content, float width)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(width));
            EditorGUILayout.PropertyField(property, new GUIContent(""));

            GUILayout.EndHorizontal();
        }

        public static void DrawPropertyPlainCW(SerializedProperty property, GUISkin skin, string content, float width)
        {
            GUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(content), skin.FindStyle("Text"), GUILayout.Width(width));
            EditorGUILayout.PropertyField(property, new GUIContent(""));

            GUILayout.EndHorizontal();
        }

        public static int DrawTabs(int tabIndex, GUIContent[] tabs, GUISkin skin)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(17);

            tabIndex = GUILayout.Toolbar(tabIndex, tabs, skin.FindStyle("Tab Indicator"));

            GUILayout.EndHorizontal();
            GUILayout.Space(-40);
            GUILayout.BeginHorizontal();
            GUILayout.Space(17);

            return tabIndex;
        }

        public static void DrawComponentHeader(GUISkin skin, string content)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Box(new GUIContent(""), skin.FindStyle(content));
            GUILayout.EndHorizontal();
            GUILayout.Space(-42);
        }

        public static void DrawHeader(GUISkin skin, string content, int space)
        {
            GUILayout.Space(space);
            GUILayout.Box(new GUIContent(""), skin.FindStyle(content));
        }

        public static bool DrawToggle(bool value, GUISkin skin, string content)
        {
            GUILayout.BeginHorizontal(EditorStyles.helpBox);

            value = GUILayout.Toggle(value, new GUIContent(content), skin.FindStyle("Toggle"));
            value = GUILayout.Toggle(value, new GUIContent(""), skin.FindStyle("Toggle Helper"));

            GUILayout.EndHorizontal();
            return value;
        }

        public static bool DrawTogglePlain(bool value, GUISkin skin, string content)
        {
            GUILayout.BeginHorizontal();

            value = GUILayout.Toggle(value, new GUIContent(content), skin.FindStyle("Toggle"));
            value = GUILayout.Toggle(value, new GUIContent(""), skin.FindStyle("Toggle Helper"));

            GUILayout.EndHorizontal();
            return value;
        }

        public static void DrawUIManagerConnectedHeader()
        {
            EditorGUILayout.HelpBox("This object is connected with the UI Manager. Some parameters (such as colors, " +
                               "fonts or booleans) are managed by the manager.", MessageType.Info);
        }

        public static void DrawUIManagerPresetHeader()
        {
            EditorGUILayout.HelpBox("This object is subject to a custom preset and cannot be used with the UI Manager. " +
                                         "You can use the standard preset for UI Manager connection.", MessageType.Info);
        }

        public static void DrawUIManagerDisconnectedHeader()
        {
            EditorGUILayout.HelpBox("This object does not have any connection with the UI Manager.", MessageType.Info);
        }
    }
}
#endif