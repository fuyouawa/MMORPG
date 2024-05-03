using UnityEngine;
using System.Collections.Generic;
using UnityEditor;

[CustomPropertyDrawer(typeof(EnumConditionAttribute))]
public class EnumConditionAttributeDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		EnumConditionAttribute enumConditionAttribute = (EnumConditionAttribute)attribute;
		bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);
		bool previouslyEnabled = GUI.enabled;
		GUI.enabled = enabled;
		if (!enumConditionAttribute.Hidden || enabled)
		{
			EditorGUI.PropertyField(position, property, label, true);
		}
		GUI.enabled = previouslyEnabled;
	}

	private static Dictionary<string, string> cachedPaths = new Dictionary<string, string>();

	private bool GetConditionAttributeResult(EnumConditionAttribute enumConditionAttribute, SerializedProperty property)
	{
		bool enabled = true;

		SerializedProperty enumProp;
		string enumPropPath = string.Empty;
		string propertyPath = property.propertyPath;

		if (!cachedPaths.TryGetValue(propertyPath, out enumPropPath))
		{
			enumPropPath = propertyPath.Replace(property.name, enumConditionAttribute.ConditionEnum);
			cachedPaths.Add(propertyPath, enumPropPath);
		}

		enumProp = property.serializedObject.FindProperty(enumPropPath);

		if (enumProp != null)
		{
			int currentEnum = enumProp.enumValueIndex;
			enabled = enumConditionAttribute.ContainsBitFlag(currentEnum);
		}
		else
		{
			Debug.LogWarning("No matching boolean found for ConditionAttribute in object: " + enumConditionAttribute.ConditionEnum);
		}

		return enabled;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		EnumConditionAttribute enumConditionAttribute = (EnumConditionAttribute)attribute;
		bool enabled = GetConditionAttributeResult(enumConditionAttribute, property);

		if (!enumConditionAttribute.Hidden || enabled)
		{
			return EditorGUI.GetPropertyHeight(property, label);
		}
		else
		{
			return -EditorGUIUtility.standardVerticalSpacing;
		}
	}
}
