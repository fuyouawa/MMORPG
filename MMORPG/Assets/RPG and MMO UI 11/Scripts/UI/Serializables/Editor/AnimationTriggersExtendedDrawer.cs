using UnityEngine;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CustomPropertyDrawer(typeof(AnimationTriggersExtended), true)]
	public class AnimationTriggersExtendedDrawer : PropertyDrawer
	{
		public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
		{
			Rect position = rect;
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty property = prop.FindPropertyRelative("m_NormalTrigger");
			SerializedProperty property2 = prop.FindPropertyRelative("m_HighlightedTrigger");
			SerializedProperty property3 = prop.FindPropertyRelative("m_PressedTrigger");
			SerializedProperty property4 = prop.FindPropertyRelative("m_ActiveTrigger");
			SerializedProperty property5 = prop.FindPropertyRelative("m_ActiveHighlightedTrigger");
			SerializedProperty property6 = prop.FindPropertyRelative("m_ActivePressedTrigger");
			SerializedProperty property7 = prop.FindPropertyRelative("m_DisabledTrigger");
			
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
			EditorGUI.PropertyField(position, property7);
		}
		
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return 7f * EditorGUIUtility.singleLineHeight + 6f * EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
