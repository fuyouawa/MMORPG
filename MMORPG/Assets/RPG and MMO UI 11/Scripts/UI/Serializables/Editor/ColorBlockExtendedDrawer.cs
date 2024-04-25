using UnityEngine;
using UnityEditor;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CustomPropertyDrawer(typeof(ColorBlockExtended), true)]
	public class ColorBlockExtendedDrawer : PropertyDrawer
	{
		protected static ColorBlockExtended m_Copy;
		protected static bool m_HasCopy = false;
		
		public override void OnGUI(Rect rect, SerializedProperty prop, GUIContent label)
		{
			Rect position = rect;
			position.height = EditorGUIUtility.singleLineHeight;
			SerializedProperty property = prop.FindPropertyRelative("m_NormalColor");
			SerializedProperty property2 = prop.FindPropertyRelative("m_HighlightedColor");
			SerializedProperty property3 = prop.FindPropertyRelative("m_PressedColor");
			SerializedProperty property4 = prop.FindPropertyRelative("m_ActiveColor");
			SerializedProperty property5 = prop.FindPropertyRelative("m_ActiveHighlightedColor");
			SerializedProperty property6 = prop.FindPropertyRelative("m_ActivePressedColor");
			SerializedProperty property7 = prop.FindPropertyRelative("m_DisabledColor");
			SerializedProperty property8 = prop.FindPropertyRelative("m_ColorMultiplier");
			SerializedProperty property9 = prop.FindPropertyRelative("m_FadeDuration");
			
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
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property8);
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
			EditorGUI.PropertyField(position, property9);
			
			Rect controlRect = EditorGUILayout.GetControlRect();
			controlRect.xMin = (controlRect.xMin + EditorGUIUtility.labelWidth);
			
			// Copy button
			if (GUI.Button(new Rect(controlRect.x, controlRect.y, ((controlRect.width / 2f) - 2f), controlRect.height), "Copy", EditorStyles.miniButton))
			{
				// Save the current values
				ColorBlockExtendedDrawer.Copy(prop);
			}
			
			// Disable the paste button if we dont have a copied property
			if (!m_HasCopy)
				GUI.enabled = false;
			
			if (GUI.Button(new Rect((controlRect.x + ((controlRect.width / 2f) + 4f)), controlRect.y, ((controlRect.width / 2f) - 2f), controlRect.height), "Paste", EditorStyles.miniButton))
			{
				// Apply the copied values
				ColorBlockExtendedDrawer.Paste(ref prop);
			}
			GUI.enabled = true;
		}
		
		public override float GetPropertyHeight(SerializedProperty prop, GUIContent label)
		{
			return 9f * EditorGUIUtility.singleLineHeight + 8f * EditorGUIUtility.standardVerticalSpacing;
		}
		
		protected static void Copy(SerializedProperty prop)
		{
			m_Copy = new ColorBlockExtended();
			m_Copy.normalColor = prop.FindPropertyRelative("m_NormalColor").colorValue;
			m_Copy.highlightedColor = prop.FindPropertyRelative("m_HighlightedColor").colorValue;
			m_Copy.pressedColor = prop.FindPropertyRelative("m_PressedColor").colorValue;
			m_Copy.activeColor = prop.FindPropertyRelative("m_ActiveColor").colorValue;
			m_Copy.activeHighlightedColor = prop.FindPropertyRelative("m_ActiveHighlightedColor").colorValue;
			m_Copy.activePressedColor = prop.FindPropertyRelative("m_ActivePressedColor").colorValue;
			m_Copy.disabledColor = prop.FindPropertyRelative("m_DisabledColor").colorValue;
			m_Copy.colorMultiplier = prop.FindPropertyRelative("m_ColorMultiplier").floatValue;
			m_Copy.fadeDuration = prop.FindPropertyRelative("m_FadeDuration").floatValue;
			
			m_HasCopy = true;
		}
		
		protected static void Paste(ref SerializedProperty prop)
		{
			if (!m_HasCopy)
				return;
				
			prop.FindPropertyRelative("m_NormalColor").colorValue = m_Copy.normalColor;
			prop.FindPropertyRelative("m_HighlightedColor").colorValue = m_Copy.highlightedColor;
			prop.FindPropertyRelative("m_PressedColor").colorValue = m_Copy.pressedColor;
			prop.FindPropertyRelative("m_ActiveColor").colorValue = m_Copy.activeColor;
			prop.FindPropertyRelative("m_ActiveHighlightedColor").colorValue = m_Copy.activeHighlightedColor;
			prop.FindPropertyRelative("m_ActivePressedColor").colorValue = m_Copy.activePressedColor;
			prop.FindPropertyRelative("m_DisabledColor").colorValue = m_Copy.disabledColor;
			prop.FindPropertyRelative("m_ColorMultiplier").floatValue = m_Copy.colorMultiplier;
			prop.FindPropertyRelative("m_FadeDuration").floatValue = m_Copy.fadeDuration;
		}
	}
}
