#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(IconManager))]
    public class IconManagerEditor : Editor
    {
        GUISkin customSkin;
        private IconManager imTarget;
        private int currentTab;
        private int tempSizeIndex;
        private string tempSizeID;
        private string searchText;
        private string defaultSize = "128x";

        protected GUIStyle panelStyle;
        protected GUIStyle lipStyle;
        protected GUIStyle lipAltStyle;
        Vector2 scrollPosition = Vector2.zero;
        List<string> sizeList = new List<string>();

        private void OnEnable()
        {
            imTarget = (IconManager)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }

            sizeList.Clear();
            if (imTarget.size32 == true) { sizeList.Add("32x"); }
            if (imTarget.size64 == true) { sizeList.Add("64x"); }
            if (imTarget.size128 == true) { sizeList.Add("128x"); }
            if (imTarget.size256 == true) { sizeList.Add("256x"); }

            for (int i = 0; i < sizeList.Count; i++)
            {
                if (sizeList[i].ToString() == imTarget.currentSize)
                    tempSizeIndex = i;
            }
        }

        void UpdateIconProperties()
        {
            sizeList.Clear();
            if (imTarget.size32 == true) { sizeList.Add("32x"); }
            if (imTarget.size64 == true) { sizeList.Add("64x"); }
            if (imTarget.size128 == true) { sizeList.Add("128x"); }
            if (imTarget.size256 == true) { sizeList.Add("256x"); }

            for (int i = 0; i < sizeList.Count; i++)
            {
                if (sizeList[i].ToString() == imTarget.currentSize)
                    tempSizeIndex = i;
            }

            ConvertIDtoIndex();
            imTarget.enabled = false;
            imTarget.enabled = true;
            imTarget.UpdateElement();
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        void ConvertIDtoIndex()
        {
            if (tempSizeID == "32x") { imTarget.spriteSize = 0; }
            else if (tempSizeID == "64x") { imTarget.spriteSize = 1; }
            else if (tempSizeID == "128x") { imTarget.spriteSize = 2; }
            else if (tempSizeID == "256x") { imTarget.spriteSize = 3; }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "IM Top Header");

            Color defaultColor = GUI.color;

            GUIContent[] toolbarTabs = new GUIContent[2];
            toolbarTabs[0] = new GUIContent("Content");
            toolbarTabs[1] = new GUIContent("Settings");

            currentTab = MUIPEditorHandler.DrawTabs(currentTab, toolbarTabs, customSkin);

            if (GUILayout.Button(new GUIContent("Content", "Content"), customSkin.FindStyle("Tab Content")))
                currentTab = 0;
            if (GUILayout.Button(new GUIContent("Settings", "Settings"), customSkin.FindStyle("Tab Settings")))
                currentTab = 1;

            GUILayout.EndHorizontal();

            var selectedIconID = serializedObject.FindProperty("selectedIconID");
            var currentSize = serializedObject.FindProperty("currentSize");
            var iconLibrary = serializedObject.FindProperty("iconLibrary");

            // Custom panel
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelStyle.margin = new RectOffset(0, 0, 0, 0);
            panelStyle.padding = new RectOffset(3, 4, 3, 4);

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(iconLibrary, customSkin, "Icon Library");

                    if (imTarget.iconLibrary == null)
                    {
                        serializedObject.ApplyModifiedProperties();
                        return;
                    }

                    if (imTarget.iconLibrary.icons.Count == 0)
                    {
                        EditorGUILayout.HelpBox("There are no items in the selected icon library.", MessageType.Info);
                        return;
                    }

                    if (selectedIconID.stringValue == "")
                        EditorGUILayout.HelpBox("No icon selected.", MessageType.Info);
                  
                    else
                    {
                        GUILayout.BeginVertical(EditorStyles.helpBox);
                        GUILayout.BeginHorizontal();

                        GUILayout.Box(imTarget.iconLibrary.icons[imTarget.selectedIconIndex].iconPreview, customSkin.FindStyle("Icon Manager Preview"));

                        GUILayout.BeginVertical();
                        GUILayout.Space(1);
                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(new GUIContent("Selected Icon"), customSkin.FindStyle("Text"), GUILayout.Width(80));
                        GUI.enabled = false;
                        EditorGUILayout.PropertyField(selectedIconID, new GUIContent(""));
                        GUI.enabled = true;

                        GUILayout.EndHorizontal();
                        GUILayout.Space(1);
                        GUILayout.BeginHorizontal();

                        EditorGUILayout.LabelField(new GUIContent("Sprite Size"), customSkin.FindStyle("Text"), GUILayout.Width(80));
                        tempSizeIndex = EditorGUILayout.Popup(tempSizeIndex, sizeList.ToArray());

                        try { tempSizeID = sizeList[tempSizeIndex].ToString(); currentSize.stringValue = tempSizeID; }
                        catch { tempSizeID = defaultSize; currentSize.stringValue = tempSizeID; tempSizeIndex = 2; }

                        ConvertIDtoIndex();

                        if (GUILayout.Button("Refresh", GUILayout.Width(56)))
                            UpdateIconProperties();

                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }

                    GUILayout.Space(10);
                    GUILayout.Box(new GUIContent(""), customSkin.FindStyle("Customization Header"));

                    // Search field
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    GUILayout.BeginHorizontal();

                    GUILayout.Box(imTarget.iconLibrary.searchIcon, customSkin.FindStyle("Icon Manager Search"));
                    // EditorGUILayout.LabelField(new GUIContent("Search:"), customSkin.FindStyle("Text"), GUILayout.Width(50), GUILayout.Height(20));
                    searchText = EditorGUILayout.TextField(searchText, GUILayout.Height(20));

                    if (searchText != null && GUILayout.Button("╳", GUILayout.Width(19))) { searchText = null; }

                    GUILayout.Space(1);
                    GUILayout.EndHorizontal();
                    GUILayout.Space(2);
                    GUILayout.EndVertical();
                    GUILayout.Space(2);

                    // Scroll panel
                    scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(250));
                    GUILayout.BeginVertical(panelStyle);

                    if (searchText == null || searchText == "")
                    {
                        for (int i = 0; i < imTarget.iconLibrary.icons.Count; i++)
                        {
                            GUILayout.BeginHorizontal(GUILayout.Height(32));
                            GUILayout.Box(imTarget.iconLibrary.icons[i].iconPreview, customSkin.FindStyle("Icon Manager Item"));

                            GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                            if (GUILayout.Button(imTarget.iconLibrary.icons[i].iconTitle, GUILayout.Height(32)))
                            {
                                sizeList.Clear();

                                if (imTarget.iconLibrary.icons[i].iconSprite32 != null) { imTarget.size32 = true; sizeList.Add("32x"); }
                                else { imTarget.size32 = false; imTarget.spriteSize = 1; }

                                if (imTarget.iconLibrary.icons[i].iconSprite64 != null) { imTarget.size64 = true; sizeList.Add("64x"); }
                                else { imTarget.size64 = false; imTarget.spriteSize = 2; }

                                if (imTarget.iconLibrary.icons[i].iconSprite128 != null) { imTarget.size128 = true; sizeList.Add("128x"); }
                                else { imTarget.size128 = false; imTarget.spriteSize = 3; }

                                if (imTarget.iconLibrary.icons[i].iconSprite256 != null) { imTarget.size256 = true; sizeList.Add("256x"); }
                                else { imTarget.size256 = false; }

                                imTarget.selectedIconIndex = i;
                                imTarget.selectedIconID = imTarget.iconLibrary.icons[i].iconTitle;
                                UpdateIconProperties();
                            }

                            GUILayout.EndHorizontal();
                            GUILayout.Space(2);
                        }
                    }

                    else
                    {
                        for (int i = 0; i < imTarget.iconLibrary.icons.Count; i++)
                        {
                            if (imTarget.iconLibrary.icons[i].iconTitle.ToLower().Contains(searchText))
                            {
                                GUILayout.BeginHorizontal(GUILayout.Height(32));
                                GUILayout.Box(imTarget.iconLibrary.icons[i].iconPreview, customSkin.FindStyle("Icon Manager Item"));

                                GUI.skin.button.alignment = TextAnchor.MiddleLeft;
                                if (GUILayout.Button(imTarget.iconLibrary.icons[i].iconTitle, GUILayout.Height(32)))
                                {
                                    sizeList.Clear();

                                    if (imTarget.iconLibrary.icons[i].iconSprite32 != null) { imTarget.size32 = true; sizeList.Add("32x"); }
                                    else { imTarget.size32 = false; imTarget.spriteSize = 1; }

                                    if (imTarget.iconLibrary.icons[i].iconSprite64 != null) { imTarget.size64 = true; sizeList.Add("64x"); }
                                    else { imTarget.size64 = false; imTarget.spriteSize = 2; }

                                    if (imTarget.iconLibrary.icons[i].iconSprite128 != null) { imTarget.size128 = true; sizeList.Add("128x"); }
                                    else { imTarget.size128 = false; imTarget.spriteSize = 3; }

                                    if (imTarget.iconLibrary.icons[i].iconSprite256 != null) { imTarget.size256 = true; sizeList.Add("256x"); }
                                    else { imTarget.size256 = false; }

                                    imTarget.selectedIconIndex = i;
                                    imTarget.selectedIconID = imTarget.iconLibrary.icons[i].iconTitle;
                                    UpdateIconProperties();
                                }

                                GUILayout.EndHorizontal();
                                GUILayout.Space(2);
                            }
                        }
                    }

                    // Scroll Panel End
                    GUILayout.EndVertical();
                    GUILayout.EndScrollView();

                    if (GUI.enabled == true) { Repaint(); }
                    break;

                case 1:
                    GUILayout.Space(6);
                    GUILayout.Box(new GUIContent(""), customSkin.FindStyle("Options Header"));

                    if (imTarget.iconLibrary == null) { GUI.enabled = false; }
                    else { GUI.enabled = true; }

                    if (GUILayout.Button("Sort Library By Name (A to Z)")) { imTarget.iconLibrary.icons.Sort(SortByNameAtoZ); }
                    if (GUILayout.Button("Sort Library By Name (Z to A)")) { imTarget.iconLibrary.icons.Sort(SortByNameZtoA); }
                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }

        private static int SortByNameAtoZ(IconLibrary.IconItem o1, IconLibrary.IconItem o2)
        {
            // Compare the names and sort by A to Z
            return o1.iconTitle.CompareTo(o2.iconTitle);
        }

        private static int SortByNameZtoA(IconLibrary.IconItem o1, IconLibrary.IconItem o2)
        {
            // Compare the names and sort by Z to A
            return o2.iconTitle.CompareTo(o1.iconTitle);
        }
    }
}
#endif