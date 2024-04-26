using UnityEngine;
using DuloGames.UI;
using UnityEditor;
using System.Collections.Generic;

namespace DuloGamesEditor.UI
{
	[CustomEditor(typeof(UIStepBar), true)]
	public class UIStepBarEditor : Editor {
		
		private SerializedProperty m_CurrentStep;
		private SerializedProperty m_StepsCount;
		private SerializedProperty m_FillImage;
		private SerializedProperty m_BubbleRect;
		private SerializedProperty m_BubbleOffset;
		private SerializedProperty m_BubbleText;
		private SerializedProperty m_StepsGridPadding;
		private SerializedProperty m_SeparatorSprite;
		private SerializedProperty m_SeparatorSpriteActive;
		private SerializedProperty m_SeparatorSpriteColor;
		private SerializedProperty m_SeparatorAutoSize;
		private SerializedProperty m_SeparatorSize;
		
		protected virtual void OnEnable()
		{
			this.m_CurrentStep = base.serializedObject.FindProperty("m_CurrentStep");
			this.m_StepsCount = base.serializedObject.FindProperty("m_StepsCount");
			this.m_FillImage = base.serializedObject.FindProperty("m_FillImage");
			this.m_BubbleRect = base.serializedObject.FindProperty("m_BubbleRect");
			this.m_BubbleOffset = base.serializedObject.FindProperty("m_BubbleOffset");
			this.m_BubbleText = base.serializedObject.FindProperty("m_BubbleText");
			this.m_StepsGridPadding = base.serializedObject.FindProperty("m_StepsGridPadding");
			this.m_SeparatorSprite = base.serializedObject.FindProperty("m_SeparatorSprite");
			this.m_SeparatorSpriteActive = base.serializedObject.FindProperty("m_SeparatorSpriteActive");
			this.m_SeparatorSpriteColor = base.serializedObject.FindProperty("m_SeparatorSpriteColor");
			this.m_SeparatorAutoSize = base.serializedObject.FindProperty("m_SeparatorAutoSize");
			this.m_SeparatorSize = base.serializedObject.FindProperty("m_SeparatorSize");
		}
		
		public override void OnInspectorGUI()
		{
			base.serializedObject.Update();
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("General Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.IntSlider(this.m_CurrentStep, 0, this.m_StepsCount.intValue + 1, new GUIContent("Starting Step"));
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_StepsCount, new GUIContent("Step Count"));
			if (EditorGUI.EndChangeCheck())
			{
				base.serializedObject.ApplyModifiedProperties();
				(base.target as UIStepBar).ValidateOverrideFillList();
                (base.target as UIStepBar).RebuildSteps_Editor();
				base.serializedObject.Update();
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Grid Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_StepsGridPadding, new GUIContent("Padding"), true);
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Separator Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_SeparatorSprite, new GUIContent("Normal Sprite"), true);
			if (this.m_SeparatorSprite.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(this.m_SeparatorSpriteActive, new GUIContent("Active Sprite"));
				EditorGUILayout.PropertyField(this.m_SeparatorSpriteColor, new GUIContent("Sprite Color"), true);
				EditorGUILayout.PropertyField(this.m_SeparatorAutoSize, new GUIContent("Auto Size"), true);
				GUI.enabled = !this.m_SeparatorAutoSize.boolValue;
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(this.m_SeparatorSize, new GUIContent("Size"), true);
                if (EditorGUI.EndChangeCheck())
                {
                    base.serializedObject.ApplyModifiedProperties();
                    (base.target as UIStepBar).RebuildSteps_Editor();
                    base.serializedObject.Update();
                }
                GUI.enabled = true;
			}
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Fill Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_FillImage, new GUIContent("Image"));
			this.DrawOverrideFillTable();
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			
			EditorGUILayout.LabelField("Bubble Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_BubbleRect, new GUIContent("Rect Transform"));
			EditorGUILayout.PropertyField(this.m_BubbleText, new GUIContent("Text"));
			EditorGUILayout.PropertyField(this.m_BubbleOffset, new GUIContent("Offset"));
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
			
			EditorGUILayout.Space();
			base.serializedObject.ApplyModifiedProperties();
		}
		
		protected void DrawOverrideFillTable()
		{
			UIStepBar bar = base.target as UIStepBar;
			List<UIStepBar.StepFillInfo> list = bar.GetOverrideFillList();
			
			EditorGUILayout.LabelField("Override Fill Amount");
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			
			for (int i = 1; i <= this.m_StepsCount.intValue; i++)
			{
				// Check if we have override info for the step
				int overrideIndex = list.FindIndex(x => x.index == i);
				
				// If we have info
				if (overrideIndex >= 0)
				{
					// Get the info
					UIStepBar.StepFillInfo info = list[overrideIndex];
					
					EditorGUI.BeginChangeCheck();
					float newAmount = EditorGUILayout.FloatField("Step #" + i.ToString(), info.amount);
					if (EditorGUI.EndChangeCheck())
					{
                        Undo.RecordObject(target, "UI Step Bar changed.");

                        UIStepBar.StepFillInfo newInfo = new UIStepBar.StepFillInfo();
						newInfo.amount = newAmount;
						newInfo.index = i;
						list[overrideIndex] = newInfo;
						
						// Validate the override list to remove the zero amount info
						if (newAmount == 0f)
							bar.ValidateOverrideFillList();
						
						// Update the fill image fillAmount
						bar.UpdateFillImage();
						
						// Set the object as dirty to be saved on the disk
						EditorUtility.SetDirty(bar);
					}
				}
				else // We dont have override info for the current step
				{
					EditorGUI.BeginChangeCheck();
					float newAmount = EditorGUILayout.FloatField("Step #" + i.ToString(), bar.GetStepFillAmount(i));
					if (EditorGUI.EndChangeCheck())
					{
						if (newAmount > 0f)
						{
                            Undo.RecordObject(target, "UI Step Bar changed.");

                            UIStepBar.StepFillInfo newInfo = new UIStepBar.StepFillInfo();
							newInfo.amount = newAmount;
							newInfo.index = i;
							list.Add(newInfo);
							
							// Update the fill image fillAmount
							bar.UpdateFillImage();
							
							// Set the object as dirty to be saved on the disk
							EditorUtility.SetDirty(bar);
						}
					}
				}
			}
			
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);
		}
	}
}
