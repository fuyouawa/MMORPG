using System;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(Color))]
public class ColorPropertyDrawer : PropertyDrawer
{
	private const float hexFieldWidth = 60f;
    private const float alphaFieldWidth = 40f;
    private const float spacing = 5f;

	public override void OnGUI(Rect pos, SerializedProperty prop, GUIContent label)
	{
		label = EditorGUI.BeginProperty(pos, label, prop);

		// Draw label
		pos = EditorGUI.PrefixLabel(pos, GUIUtility.GetControlID(FocusType.Passive), label);
		
		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

        float colorWidth = (pos.width - hexFieldWidth - spacing - alphaFieldWidth - spacing);

        Color32 color = prop.colorValue;
		Color32 color2 = EditorGUI.ColorField(new Rect(pos.x, pos.y, colorWidth, pos.height), prop.colorValue);

		if (!color2.Equals(color))
			prop.colorValue = color = color2;

		string colorString = EditorGUI.TextField(new Rect((pos.x + colorWidth + spacing), pos.y, hexFieldWidth, pos.height), CommonColorBuffer.ColorToString(color));
		try
		{
			color2 = CommonColorBuffer.StringToColor(colorString);

			if (!color2.Equals(color))
				prop.colorValue = color = color2;
		}
		catch { }

        
        float newAlpha = EditorGUI.Slider(new Rect((pos.x + colorWidth + hexFieldWidth + (spacing * 2f)), pos.y, alphaFieldWidth, pos.height), prop.colorValue.a, 0f, 1f);

        if (!newAlpha.Equals(prop.colorValue.a))
            prop.colorValue = new Color(prop.colorValue.r, prop.colorValue.g, prop.colorValue.b, newAlpha);
        
        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}
