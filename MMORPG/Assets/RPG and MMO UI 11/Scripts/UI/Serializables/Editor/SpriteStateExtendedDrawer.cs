using UnityEngine;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CustomPropertyDrawer(typeof(SpriteStateExtended), true)]
	public class SpriteStateExtendedDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
		{
			Rect position = rect;
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty property = prop.FindPropertyRelative("m_HighlightedSprite");
			SerializedProperty property2 = prop.FindPropertyRelative("m_PressedSprite");
			SerializedProperty property3 = prop.FindPropertyRelative("m_ActiveSprite");
			SerializedProperty property4 = prop.FindPropertyRelative("m_ActiveHighlightedSprite");
			SerializedProperty property5 = prop.FindPropertyRelative("m_ActivePressedSprite");
			SerializedProperty property6 = prop.FindPropertyRelative("m_DisabledSprite");
			
			EditorGUI.PropertyField(position, property);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property2);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property3);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property4);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property5);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property6);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		}
		
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return 6f * EditorGUIUtility.singleLineHeight + 5f * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
