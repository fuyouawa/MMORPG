#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Michsky.MUIP
{
    [CustomEditor(typeof(ListView))]
    public class ListViewEditor : Editor
    {
        GUISkin customSkin;
        private ListView lvTarget;
        private int currentTab;

        protected GUIStyle panelStyle;
        protected GUIStyle lipStyle;
        protected GUIStyle lipAltStyle;
        Vector2 scrollPosition = Vector2.zero;

        private void OnEnable()
        {
            lvTarget = (ListView)target;

            if (EditorGUIUtility.isProSkin == true) { customSkin = MUIPEditorHandler.GetDarkEditor(customSkin); }
            else { customSkin = MUIPEditorHandler.GetLightEditor(customSkin); }
        }

        public override void OnInspectorGUI()
        {
            MUIPEditorHandler.DrawComponentHeader(customSkin, "LV Top Header");

            Color defaultColor = GUI.color;

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

            var rowCount = serializedObject.FindProperty("rowCount");
            var listItems = serializedObject.FindProperty("listItems");
            var itemParent = serializedObject.FindProperty("itemParent");
            var itemPreset = serializedObject.FindProperty("itemPreset");
            var initializeOnAwake = serializedObject.FindProperty("initializeOnAwake");
            var showScrollbar = serializedObject.FindProperty("showScrollbar");
            var scrollbar = serializedObject.FindProperty("scrollbar");

            // Foldout style
            GUIStyle foldoutStyle = customSkin.FindStyle("UIM Foldout");

            // Custom panel
            panelStyle = new GUIStyle(GUI.skin.box);
            panelStyle.normal.textColor = GUI.skin.label.normal.textColor;
            panelStyle.margin = new RectOffset(0, 0, 0, 0);
            panelStyle.padding = new RectOffset(0, 0, 0, 0);

            switch (currentTab)
            {
                case 0:
                    MUIPEditorHandler.DrawHeader(customSkin, "Content Header", 6);
                    GUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUI.indentLevel = 1;
                    EditorGUILayout.PropertyField(listItems, new GUIContent("List Items"), true);
                    EditorGUI.indentLevel = 0;

                    if (GUILayout.Button("+ Add a new list item", customSkin.button))
                    {
                        ListView.ListItem item = new ListView.ListItem();
                        lvTarget.listItems.Add(item);
                        return;
                    }

                    GUILayout.EndVertical();

                    MUIPEditorHandler.DrawHeader(customSkin, "Customization Header", 10);
                    MUIPEditorHandler.DrawProperty(rowCount, customSkin, "Row Count");

                    if (lvTarget.listItems.Count == 0) { EditorGUILayout.HelpBox("There are no items in the list. ", MessageType.Info); }
                    else
                    {
                        int tempHeight;
                        if (lvTarget.listItems.Count < 3) { tempHeight = 0; }
                        else { tempHeight = 300; }

                        // Scroll panel
                        scrollPosition = GUILayout.BeginScrollView(scrollPosition, false, true, GUIStyle.none, GUI.skin.verticalScrollbar, GUILayout.Height(tempHeight));
                        GUILayout.BeginVertical(panelStyle);

                        for (int i = 0; i < lvTarget.listItems.Count; i++)
                        {
                            // Start Item Background
                            GUILayout.BeginVertical(EditorStyles.helpBox);

                            GUILayout.Space(5);
                            GUILayout.BeginHorizontal();
                            if (string.IsNullOrEmpty(lvTarget.listItems[i].itemTitle) == true) { lvTarget.listItems[i].isExpanded = EditorGUILayout.Foldout(lvTarget.listItems[i].isExpanded, "Item #" + i.ToString(), true, foldoutStyle); }
                            else { lvTarget.listItems[i].isExpanded = EditorGUILayout.Foldout(lvTarget.listItems[i].isExpanded, lvTarget.listItems[i].itemTitle, true, foldoutStyle); }
                            lvTarget.listItems[i].isExpanded = GUILayout.Toggle(lvTarget.listItems[i].isExpanded, new GUIContent(""), customSkin.FindStyle("Toggle Helper"));
                            GUILayout.EndHorizontal();
                            GUILayout.Space(2);

                            if (lvTarget.listItems[i].isExpanded)
                            {
                                // Row 1
                                GUILayout.BeginVertical(EditorStyles.helpBox);

                                // Row 1 Type
                                GUILayout.BeginHorizontal();
                                EditorGUILayout.LabelField(new GUIContent("Row #1 Type"), customSkin.FindStyle("Text"), GUILayout.Width(90));
                                lvTarget.listItems[i].row0.rowType = (ListView.RowType)EditorGUILayout.EnumPopup(lvTarget.listItems[i].row0.rowType);
                                GUILayout.EndHorizontal();

                                // Row 1 Content
                                EditorGUI.indentLevel++;

                                if (lvTarget.listItems[i].row0.rowType == ListView.RowType.Icon)
                                {
                                    lvTarget.listItems[i].row0.rowIcon = EditorGUILayout.ObjectField(lvTarget.listItems[i].row0.rowIcon, typeof(Sprite), true) as Sprite;
                                    lvTarget.listItems[i].row0.iconScale = EditorGUILayout.FloatField("Icon Scale", lvTarget.listItems[i].row0.iconScale);
                                }

                                else if (lvTarget.listItems[i].row0.rowType == ListView.RowType.Text)
                                {
                                    lvTarget.listItems[i].row0.rowText = EditorGUILayout.TextField("Title", lvTarget.listItems[i].row0.rowText);
                                }

                                lvTarget.listItems[i].row0.usePreferredWidth = EditorGUILayout.Toggle("Use Preferred Width", lvTarget.listItems[i].row0.usePreferredWidth);
                                if (lvTarget.listItems[i].row0.usePreferredWidth == true) { lvTarget.listItems[i].row0.preferredWidth = EditorGUILayout.IntField("Preferred Width", lvTarget.listItems[i].row0.preferredWidth); }

                                EditorGUI.indentLevel--;
                                GUILayout.EndVertical();

                                // Row 2
                                if (rowCount.enumValueIndex > 0)
                                {
                                    GUILayout.BeginVertical(EditorStyles.helpBox);

                                    // Row 2 Type
                                    GUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField(new GUIContent("Row #2 Type"), customSkin.FindStyle("Text"), GUILayout.Width(90));
                                    lvTarget.listItems[i].row1.rowType = (ListView.RowType)EditorGUILayout.EnumPopup(lvTarget.listItems[i].row1.rowType);
                                    GUILayout.EndHorizontal();

                                    // Row 2 Content
                                    EditorGUI.indentLevel++;

                                    if (lvTarget.listItems[i].row1.rowType == ListView.RowType.Icon)
                                    {
                                        lvTarget.listItems[i].row1.rowIcon = EditorGUILayout.ObjectField(lvTarget.listItems[i].row1.rowIcon, typeof(Sprite), true) as Sprite;
                                        lvTarget.listItems[i].row1.iconScale = EditorGUILayout.FloatField("Icon Scale", lvTarget.listItems[i].row1.iconScale);
                                    }

                                    else if (lvTarget.listItems[i].row1.rowType == ListView.RowType.Text)
                                    {
                                        lvTarget.listItems[i].row1.rowText = EditorGUILayout.TextField("Title", lvTarget.listItems[i].row1.rowText);
                                    }

                                    lvTarget.listItems[i].row1.usePreferredWidth = EditorGUILayout.Toggle("Use Preferred Width", lvTarget.listItems[i].row1.usePreferredWidth);
                                    if (lvTarget.listItems[i].row1.usePreferredWidth == true) { lvTarget.listItems[i].row1.preferredWidth = EditorGUILayout.IntField("Preferred Width", lvTarget.listItems[i].row1.preferredWidth); }

                                    EditorGUI.indentLevel--;
                                    GUILayout.EndVertical();
                                }

                                // Row 3
                                if (rowCount.enumValueIndex > 1)
                                {
                                    GUILayout.BeginVertical(EditorStyles.helpBox);

                                    // Row 3 Type
                                    GUILayout.BeginHorizontal();
                                    EditorGUILayout.LabelField(new GUIContent("Row #3 Type"), customSkin.FindStyle("Text"), GUILayout.Width(90));
                                    lvTarget.listItems[i].row2.rowType = (ListView.RowType)EditorGUILayout.EnumPopup(lvTarget.listItems[i].row2.rowType);
                                    GUILayout.EndHorizontal();

                                    // Row 3 Content
                                    EditorGUI.indentLevel++;

                                    if (lvTarget.listItems[i].row2.rowType == ListView.RowType.Icon)
                                    {
                                        lvTarget.listItems[i].row2.rowIcon = EditorGUILayout.ObjectField(lvTarget.listItems[i].row2.rowIcon, typeof(Sprite), true) as Sprite;
                                        lvTarget.listItems[i].row2.iconScale = EditorGUILayout.FloatField("Icon Scale", lvTarget.listItems[i].row2.iconScale);
                                    }

                                    else if (lvTarget.listItems[i].row2.rowType == ListView.RowType.Text)
                                    {
                                        lvTarget.listItems[i].row2.rowText = EditorGUILayout.TextField("Title", lvTarget.listItems[i].row2.rowText);
                                    }

                                    lvTarget.listItems[i].row2.usePreferredWidth = EditorGUILayout.Toggle("Use Preferred Width", lvTarget.listItems[i].row2.usePreferredWidth);
                                    if (lvTarget.listItems[i].row2.usePreferredWidth == true) { lvTarget.listItems[i].row2.preferredWidth = EditorGUILayout.IntField("Preferred Width", lvTarget.listItems[i].row2.preferredWidth); }

                                    EditorGUI.indentLevel--;
                                    GUILayout.EndVertical();
                                }
                            }       

                            // End item
                            GUILayout.EndVertical();
                        }

                        if (GUILayout.Button("Pre-Initialize Items", customSkin.button)) { lvTarget.InitializeItems(); }

                        // Scroll Panel End
                        GUILayout.EndVertical();
                        GUILayout.EndScrollView();
                        if (GUI.enabled == true) { Repaint(); }
                    }

                    break;

                case 1:
                    MUIPEditorHandler.DrawHeader(customSkin, "Core Header", 6);
                    MUIPEditorHandler.DrawProperty(itemParent, customSkin, "Item Parent");
                    MUIPEditorHandler.DrawProperty(itemPreset, customSkin, "Item Preset");
                    MUIPEditorHandler.DrawProperty(scrollbar, customSkin, "Scrollbar");
                    break;

                case 2:
                    MUIPEditorHandler.DrawHeader(customSkin, "Options Header", 6);
                    initializeOnAwake.boolValue = MUIPEditorHandler.DrawToggle(initializeOnAwake.boolValue, customSkin, "Initialize On Awake");
                    showScrollbar.boolValue = MUIPEditorHandler.DrawToggle(showScrollbar.boolValue, customSkin, "Show Scrollbar");
                    if (GUILayout.Button("Sort List By Name (A to Z)")) { lvTarget.listItems.Sort(SortByNameAtoZ); }
                    if (GUILayout.Button("Sort List By Name (Z to A)")) { lvTarget.listItems.Sort(SortByNameZtoA); }
                    break;
            }

            if (Application.isPlaying == false) { this.Repaint(); }
            serializedObject.ApplyModifiedProperties();
        }

        private static int SortByNameAtoZ(ListView.ListItem o1, ListView.ListItem o2)
        {
            // Compare the names and sort by A to Z
            return o1.itemTitle.CompareTo(o2.itemTitle);
        }

        private static int SortByNameZtoA(ListView.ListItem o1, ListView.ListItem o2)
        {
            // Compare the names and sort by Z to A
            return o2.itemTitle.CompareTo(o1.itemTitle);
        }
    }
}
#endif