using UnityEngine;
using DuloGames.UI;
using UnityEditor;

namespace DuloGamesEditor.UI
{
	[CanEditMultipleObjects, CustomEditor(typeof(UIProgressBar), true)]
	public class UIProgressBarEditor : Editor {

        private UIProgressBar bar;

        private SerializedProperty m_Type;
		private SerializedProperty m_TargetImage;
        private SerializedProperty m_Sprites;
		private SerializedProperty m_TargetTransform;
		private SerializedProperty m_FillSizing;
		private SerializedProperty m_MinWidth;
		private SerializedProperty m_MaxWidth;
		private SerializedProperty m_FillAmount;
		private SerializedProperty m_Steps;
		private SerializedProperty m_OnChange;
		
		protected void OnEnable()
		{
            this.bar = target as UIProgressBar;

            this.m_Type = base.serializedObject.FindProperty("m_Type");
			this.m_TargetImage = base.serializedObject.FindProperty("m_TargetImage");
            this.m_Sprites = base.serializedObject.FindProperty("m_Sprites");
            this.m_TargetTransform = base.serializedObject.FindProperty("m_TargetTransform");
			this.m_FillSizing = base.serializedObject.FindProperty("m_FillSizing");
			this.m_MinWidth = base.serializedObject.FindProperty("m_MinWidth");
			this.m_MaxWidth = base.serializedObject.FindProperty("m_MaxWidth");
			this.m_FillAmount = base.serializedObject.FindProperty("m_FillAmount");
			this.m_Steps = base.serializedObject.FindProperty("m_Steps");
			this.m_OnChange = base.serializedObject.FindProperty("onChange");
		}
		
		public override void OnInspectorGUI()
		{
			bool amountChanged = false;
			
			base.serializedObject.Update();
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.LabelField("Fill Properties", EditorStyles.boldLabel);
			EditorGUI.indentLevel = (EditorGUI.indentLevel + 1);
			EditorGUILayout.PropertyField(this.m_Type, new GUIContent("Fill Type"));
			// Normal type
			if (this.m_Type.enumValueIndex == 0)
			{
				EditorGUILayout.PropertyField(this.m_TargetImage, new GUIContent("Fill Target"));
				if (this.m_TargetImage.objectReferenceValue != null && (this.m_TargetImage.objectReferenceValue as UnityEngine.UI.Image).type != UnityEngine.UI.Image.Type.Filled)
				{
					EditorGUILayout.HelpBox("The target image must be of type Filled.", MessageType.Info);
				}
			}
			else if (this.m_Type.enumValueIndex == 1)
			{
				EditorGUILayout.PropertyField(this.m_TargetTransform, new GUIContent("Fill Target"));
				EditorGUILayout.PropertyField(this.m_FillSizing, new GUIContent("Fill Sizing"));
				if (this.m_FillSizing.enumValueIndex == 1)
				{
					EditorGUILayout.PropertyField(this.m_MinWidth, new GUIContent("Min Width"));
					EditorGUILayout.PropertyField(this.m_MaxWidth, new GUIContent("Max Width"));
				}
			}
            else if (this.m_Type.enumValueIndex == 2)
            {
                EditorGUILayout.PropertyField(this.m_TargetImage, new GUIContent("Fill Target"));
                EditorGUILayout.PropertyField(this.m_Sprites, new GUIContent("Sprites"), true);
            }
			EditorGUILayout.PropertyField(this.m_Steps, new GUIContent("Steps"));
			EditorGUI.indentLevel = (EditorGUI.indentLevel - 1);

			EditorGUILayout.Separator();
			
			EditorGUI.BeginChangeCheck();
			EditorGUILayout.PropertyField(this.m_FillAmount, new GUIContent("Fill Amount"));
			if (EditorGUI.EndChangeCheck())
			{
				amountChanged = true;
			}
			
			EditorGUILayout.Separator();
			
			EditorGUILayout.PropertyField(this.m_OnChange, true);
			
			base.serializedObject.ApplyModifiedProperties();
			
			if (amountChanged)
			{
                this.bar.UpdateBarFill();
                this.bar.onChange.Invoke(this.m_FillAmount.floatValue);
			}
		}
	}
}
