using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
    [CustomPropertyDrawer(typeof(UITooltipLineStyle))]
    class UITooltipLineStyleDrawer : PropertyDrawer
    {
        private const float Spacing = 2f;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            bool displayName = property.FindPropertyRelative("DisplayName").boolValue;
            
            EditorGUI.BeginProperty(position, label, property);

            position = new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight);
            
            property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, label);
            position = new Rect(position.x, position.y + 16f + Spacing, position.width, EditorGUIUtility.singleLineHeight);

            if (property.isExpanded)
            {
                EditorGUI.indentLevel += 1;

                if (displayName)
                {
                    EditorGUI.PropertyField(position, property.FindPropertyRelative("Name"), new GUIContent("Name"));
                    position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);
                }

                EditorGUI.PropertyField(position, property.FindPropertyRelative("TextFont"), new GUIContent("Font"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("TextFontStyle"), new GUIContent("Font Style"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("TextFontSize"), new GUIContent("Font Size"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("TextFontLineSpacing"), new GUIContent("Line Spacing"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("OverrideTextAlignment"), new GUIContent("Override Text Alignment"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.PropertyField(position, property.FindPropertyRelative("TextFontColor"), new GUIContent("Font Color"));
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);
                
                EditorGUI.PropertyField(position, property.FindPropertyRelative("TextEffects"), new GUIContent("Text Effects"), true);
                position = new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight + Spacing, position.width, EditorGUIUtility.singleLineHeight);

                EditorGUI.indentLevel -= 1;
            }

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (!property.isExpanded)
                return base.GetPropertyHeight(property, label);

            bool displayName = property.FindPropertyRelative("DisplayName").boolValue;

            float effectsHeight = EditorGUI.GetPropertyHeight(property.FindPropertyRelative("TextEffects"), new GUIContent("Text Effects"), true);

            if (!displayName)
                return base.GetPropertyHeight(property, label) + 20f + (EditorGUIUtility.singleLineHeight * 5f) + (Spacing * 4f) + effectsHeight;
            
            return base.GetPropertyHeight(property, label) + 20f + (EditorGUIUtility.singleLineHeight * 6f) + (Spacing * 5f) + effectsHeight;
        }
    }
}
