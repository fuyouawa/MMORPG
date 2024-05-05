using System;
using NLog.Conditions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerConditionBinder))]
public class PlayerConditionBinderDrawer : PropertyDrawer
{
    public static readonly float LineTotalHeight = EditorGUIUtility.singleLineHeight + 0.5f;
    public static readonly float TotalHeight = LineTotalHeight;

    private SerializedProperty _methodNameProperty;
    private SerializedProperty _methodObjectProperty;

    private Rect _position;
    private MethodInfo[] _methods;
    private string[] _options;
    private int _selectedIndex;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _methodNameProperty = property.FindPropertyRelative("MethodName");
        _methodObjectProperty = property.FindPropertyRelative("MethodObject");

        _position = position;
        _position.height = EditorGUIUtility.singleLineHeight;

        var methodObjectRect = new Rect(_position)
        {
            width = _position.width / 2 - 2.5f
        };

        var methodNameRect = new Rect(_position)
        {
            width = _position.width / 2,
            x = methodObjectRect.x + methodObjectRect.width + 2.5f
        };

        _methods = GetConditionMethods(_methodObjectProperty.objectReferenceValue?.GetType());
        _options = new[] { "NONE" }.Combine(_methods.Select(x => x.Name).ToArray());

        _selectedIndex = Array.FindIndex(_methods, x => x.Name == _methodNameProperty.stringValue);
        if (_selectedIndex == -1)
        {
            _selectedIndex = 0;
            _methodNameProperty.stringValue = string.Empty;
        }
        else
        {
            _selectedIndex++;
        }

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            EditorGUI.PropertyField(methodObjectRect, _methodObjectProperty, new GUIContent());

            using var check = new EditorGUI.ChangeCheckScope();

            _selectedIndex = EditorGUI.Popup(methodNameRect, _selectedIndex, _options);
            if (check.changed)
            {
                var name = _selectedIndex == 0 ?
                    string.Empty :
                    _methods[_selectedIndex - 1].Name;
                _methodNameProperty.stringValue = name;
                property.serializedObject.ApplyModifiedProperties();
                EditorUtility.SetDirty(property.serializedObject.targetObject);
            }
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return TotalHeight;
    }

    private MethodInfo[] GetConditionMethods(System.Type type)
    {
        if (type == null)
            return Array.Empty<MethodInfo>();
        return (from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where method.HasAttribute<StateConditionAttribute>()
            select method).ToArray();
    }
}
