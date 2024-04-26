using UnityEngine;
using DuloGames.UI;
using UnityEditor;
using System.Collections.Generic;

namespace DuloGamesEditor.UI
{
    [CustomPropertyDrawer(typeof(UITooltipLineContent))]
    class UITooltipLineContentDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool isSpacer = property.FindPropertyRelative("IsSpacer").boolValue;
            UITooltipLines.LineStyle style = (UITooltipLines.LineStyle)property.FindPropertyRelative("LineStyle").enumValueIndex;

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            EditorGUI.HelpBox(new Rect(position.x + 14f, position.y, position.width - 14f, position.height), "", MessageType.None);

            // Draw label
            //EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            position = new Rect(position.x, position.y + 6f + Spacing, position.width - 8f, EditorGUIUtility.singleLineHeight);

            // Don't make child fields be indented
            EditorGUI.indentLevel += 1;

            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            if (!isSpacer)
            {
                EditorGUI.PropertyField(position, property.FindPropertyRelative("LineStyle"), new GUIContent("Style"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                if (style == UITooltipLines.LineStyle.Custom)
                {
                    if (UITooltipManager.Instance != null)
                    {
                        this.DrawCustomStyleField(position, property.FindPropertyRelative("CustomLineStyle"));
                        position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, property.FindPropertyRelative("CustomLineStyle"), new GUIContent("Style Name"));
                        position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);
                    }
                }

                EditorGUI.PropertyField(position, property.FindPropertyRelative("Content"), new GUIContent("Content"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);
            }

            EditorGUI.PropertyField(position, property.FindPropertyRelative("IsSpacer"), new GUIContent("Is Spacer ?"));
            position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

            // Set indent back to what it was
            EditorGUI.indentLevel -= 1;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            bool isSpacer = property.FindPropertyRelative("IsSpacer").boolValue;
            UITooltipLines.LineStyle style = (UITooltipLines.LineStyle)property.FindPropertyRelative("LineStyle").enumValueIndex;

            if (isSpacer)
                return base.GetPropertyHeight(property, label) + EditorGUIUtility.singleLineHeight + Spacing;

            if (style == UITooltipLines.LineStyle.Custom)
                return base.GetPropertyHeight(property, label) + (EditorGUIUtility.singleLineHeight * 4f) + (Spacing * 3f);

            return base.GetPropertyHeight(property, label) + (EditorGUIUtility.singleLineHeight * 3f) + (Spacing * 2f);
        }

        public void DrawCustomStyleField(Rect position, SerializedProperty property)
        {
            if (UITooltipManager.Instance.customStyles.Length == 0)
            {
                EditorGUI.LabelField(position, "Please add custom styles to the tooltip manager.");
                return;
            }

            int selected = 0;
            List<string> options = new List<string>();

            var index = 0;
            foreach (UITooltipLineStyle style in UITooltipManager.Instance.customStyles)
            {
                options.Add(style.Name);

                if (style.Name == property.stringValue)
                    selected = index;

                index++;
            }

            GUI.changed = false;
            int newSelected = EditorGUI.Popup(position, "Style Name", selected, options.ToArray());

            if (GUI.changed)
                property.stringValue = UITooltipManager.Instance.customStyles[newSelected].Name;
        }
    }
}
