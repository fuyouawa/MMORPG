using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using System.Collections.Generic;
using DuloGames.UI;

namespace DuloGamesEditor.UI
{
	[CustomEditor(typeof(UISliderExtended), true)]
	public class UISliderExtendedEditor : SliderEditor {
	
		public override void OnInspectorGUI()
		{
			base.OnInspectorGUI();
			this.serializedObject.Update();
			EditorGUILayout.Space();
			
			this.DrawOptionsArea();
			
			EditorGUIUtility.labelWidth = 150f;
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Options Layout", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionsPadding"), true);
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Option Image Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionSprite"), new GUIContent("Sprite"), true);
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Option Text Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextFont"), new GUIContent("Text Font"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextStyle"), new GUIContent("Text Font Style"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextSize"), new GUIContent("Text Font Size"));
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextColor"), new GUIContent("Text Color"));
			
			UISliderExtended.TextEffectType effect = (UISliderExtended.TextEffectType)this.serializedObject.FindProperty("m_OptionTextEffect").enumValueIndex;
			
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextEffect"), new GUIContent("Text Effect"));
			
			if (effect != UISliderExtended.TextEffectType.None)
			{
				EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextEffectColor"), new GUIContent("Color"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextEffectDistance"), new GUIContent("Distance"), true);
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextEffectUseGraphicAlpha"), new GUIContent("Use graphic alpha"));
				EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			}
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTextOffset"), new GUIContent("Text Offset"), true);
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			EditorGUILayout.LabelField("Option Transition", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTransition"), new GUIContent("Text Transition"));
			
			UISliderExtended.OptionTransition textTransition = (UISliderExtended.OptionTransition)this.serializedObject.FindProperty("m_OptionTransition").enumValueIndex;
			if (textTransition == UISliderExtended.OptionTransition.ColorTint)
			{
				EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTransitionTarget"), new GUIContent("Target"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTransitionColorNormal"), new GUIContent("Color Normal"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTransitionColorActive"), new GUIContent("Color Active"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTransitionMultiplier"), new GUIContent("Color Multiplier"));
				EditorGUILayout.PropertyField(this.serializedObject.FindProperty("m_OptionTransitionDuration"), new GUIContent("Duration"));
				EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUIUtility.labelWidth = 120f;
			this.serializedObject.ApplyModifiedProperties();
			
			// Detect change in the text effect
			if (!effect.Equals((UISliderExtended.TextEffectType)this.serializedObject.FindProperty("m_OptionTextEffect").enumValueIndex))
                foreach (Object target in this.targets)
                    (target as UISliderExtended).RebuildTextEffects();
		}
		
		/// <summary>
		/// Draws the options area.
		/// </summary>
		private void DrawOptionsArea()
		{
			UISliderExtended slider = (this.target as UISliderExtended);
			List<string> newOptions = new List<string>();
			
			// Place a label for the options
			EditorGUILayout.LabelField("Options", EditorStyles.boldLabel);
			
			// Prepare the string to be used in the text area
			string text = "";
			foreach (string s in slider.options)
				text += s + "\n";
			
			string modified = EditorGUILayout.TextArea(text, GUI.skin.textArea, GUILayout.Height(100f));
			
			// Check if the options have changed
			if (!modified.Equals(text))
			{
				Undo.RecordObject(target, "UI Slider Extended changed.");
				
				string[] split = modified.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
				
				foreach (string s in split)
					newOptions.Add(s);
				
				// Apply the new list
				slider.options = newOptions;
				
				EditorUtility.SetDirty(target);
            }
		}
	}
}
