using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomPropertyDrawer(typeof(ResourcePathAttribute))]
public class ResourcePathDrawer : PropertyDrawer
{
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Now draw the property as a Slider or an IntSlider based on whether it's a float or integer.
		if (property.propertyType == SerializedPropertyType.String)
		{
			EditorGUI.PropertyField(position, property, label);
			this.DragAndDropArea(position, property);
		}
		else
		{
			EditorGUI.LabelField(position, label.text, "ResourcePath requires string.");
		}
	}
	
	private void DragAndDropArea(Rect position, SerializedProperty property)
	{
		Event evt = Event.current;
		Rect drop_area = new Rect((position.x + EditorGUIUtility.labelWidth), position.y, (EditorGUIUtility.currentViewWidth - 20f - EditorGUIUtility.labelWidth), EditorGUIUtility.singleLineHeight);
		
		//GUIStyle boxStyle = new GUIStyle(GUI.skin.box);
		//boxStyle.alignment = TextAnchor.MiddleCenter;
		//GUI.Box(drop_area, "Drop Prefab Here", boxStyle);
		
		switch (evt.type)
		{
			case EventType.DragUpdated:
			case EventType.DragPerform:
			{
				if (!drop_area.Contains(evt.mousePosition))
					return;
				
				DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
				
				if (evt.type == EventType.DragPerform)
				{
					DragAndDrop.AcceptDrag();
					
					if (DragAndDrop.objectReferences == null || DragAndDrop.objectReferences.Length == 0)
						return;
					
					string path = AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]);
					
					// Check if it's in the resources folder
					if (path.IndexOf("Resources/") == -1)
					{
						Debug.LogWarning("The given prefab is not in the resources folder!");
						return;
					}
					
					// Apply to the prefab field
					path = path.Substring(path.IndexOf("Resources/") + 10);
					
					// Remove asset extension
					if (path.IndexOf(".") > -1)
						path = path.Substring(0, path.LastIndexOf("."));
					
					// Apply to the string property
					property.stringValue = path;
				}
				break;
			}
		}
	}
}