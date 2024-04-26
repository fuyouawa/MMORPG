using UnityEngine;
using UnityEditor;
using System;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
    [CustomEditor(typeof(UISwitchSelect), true)]
    public class UISwitchSelectEditor : Editor
    {
        private SerializedProperty m_Text;
        private SerializedProperty m_PrevButton;
        private SerializedProperty m_NextButton;
        private SerializedProperty m_OnChange;

        protected void OnEnable()
        {
            this.m_Text = base.serializedObject.FindProperty("m_Text");
            this.m_PrevButton = base.serializedObject.FindProperty("m_PrevButton");
            this.m_NextButton = base.serializedObject.FindProperty("m_NextButton");
            this.m_OnChange = base.serializedObject.FindProperty("onChange");
        }

        public override void OnInspectorGUI()
        {
            UISwitchSelect select = (this.target as UISwitchSelect);
            this.serializedObject.Update();

            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(this.m_Text, new GUIContent("Label Text"));
            EditorGUILayout.PropertyField(this.m_PrevButton, new GUIContent("Prev Button"));
            EditorGUILayout.PropertyField(this.m_NextButton, new GUIContent("Next Button"));

            EditorGUILayout.Separator();
            this.DrawOptionsArea();

            EditorGUILayout.Separator();
            UISelectFieldEditor.DrawStringPopup("Selected option", select.options.ToArray(), select.value, OnDefaultOptionSelected);
        
            EditorGUILayout.Separator();
            EditorGUILayout.PropertyField(this.m_OnChange);

            this.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Raises the default option selected event.
        /// </summary>
        /// <param name="value">Value.</param>
        private void OnDefaultOptionSelected(string value)
        {
            UISwitchSelect select = (this.target as UISwitchSelect);

            Undo.RecordObject(select, "Select Field default option changed.");
            select.SelectOption(value);
            EditorUtility.SetDirty(select);
        }

        /// <summary>
        /// Draws the options area.
        /// </summary>
        private void DrawOptionsArea()
        {
            UISwitchSelect select = (this.target as UISwitchSelect);

            // Place a label for the options
            EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);

            // Prepare the string to be used in the text area
            string text = "";
            foreach (string s in select.options)
                text += s + "\n";

            string modified = EditorGUILayout.TextArea(text, GUI.skin.textArea, GUILayout.Height(100f));

            // Check if the options have changed
            if (!modified.Equals(text))
            {
                Undo.RecordObject(target, "UI Select Field changed.");

                string[] split = modified.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

                select.options.Clear();

                foreach (string s in split)
                    select.options.Add(s);

                if (string.IsNullOrEmpty(select.value) || !select.options.Contains(select.value))
                {
                    select.value = select.options.Count > 0 ? select.options[0] : "";
                }

                EditorUtility.SetDirty(target);
            }
        }
        
        /// <summary>
        /// Draws a string popup field.
        /// </summary>
        /// <param name="label">Label.</param>
        /// <param name="list">Array of values.</param>
        /// <param name="selected">The selected value.</param>
        /// <param name="onChange">On change.</param>
        public static void DrawStringPopup(string label, string[] list, string selected, Action<string> onChange)
        {
            string newValue = string.Empty;
            GUI.changed = false;

            if (list != null && list.Length > 0)
            {
                int index = 0;

                // Make sure we have a selection
                if (string.IsNullOrEmpty(selected))
                    selected = list[0];

                // Find the index of the selection
                else if (!string.IsNullOrEmpty(selected))
                {
                    for (int i = 0; i < list.Length; ++i)
                    {
                        if (selected.Equals(list[i], System.StringComparison.OrdinalIgnoreCase))
                        {
                            index = i;
                            break;
                        }
                    }
                }

                // Draw the sprite selection popup
                index = string.IsNullOrEmpty(label) ? EditorGUILayout.Popup(index, list) : EditorGUILayout.Popup(label, index, list);

                // Save the selected value
                newValue = list[index];
            }

            // Invoke the event with the selected value
            if (GUI.changed)
                onChange.Invoke(newValue);
        }
    }
}
