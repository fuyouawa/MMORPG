using System;
using System.Linq;
using System.Reflection;
using QFramework;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(CharacterAction))]
public class CharacterActionDrawer : PropertyDrawer
{
    private CharacterAction _thisAction;
    private CharacterAction[] _actions;
    private int _selectedIndex = 0;
    private string[] _options;
    private Rect _position;
    private float _totalHeight;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            _position = position;
            _position.height = EditorGUIUtility.singleLineHeight;
            _totalHeight = _position.height;

            RefreshValue(property);
            DrawDropdownSelections(property);
            NextLine();
            EditorGUI.PropertyField(_position, property, label);
            DrawExtraParams(property);
        }
    }

    private void RefreshValue(SerializedProperty property)
    {
        _thisAction = property.objectReferenceValue as CharacterAction;
        _actions = (property.serializedObject.targetObject as CharacterBrain).GetAttachedActions();
        _options = new string[_actions.Length + 1];
        _options[0] = "None";
        _selectedIndex = 0;
        int i = 1;
        foreach (var cond in _actions)
        {
            _options[i] = $"{i} - {cond.GetType()}";
            if (cond == _thisAction)
                _selectedIndex = i;
            i++;
        }
    }

    private void DrawDropdownSelections(SerializedProperty property)
    {
        EditorGUI.BeginChangeCheck();
        _selectedIndex = EditorGUI.Popup(_position, _selectedIndex, _options);
        if (EditorGUI.EndChangeCheck())
        {
            _thisAction = (_selectedIndex == 0) ? null : _actions[_selectedIndex - 1];
            property.objectReferenceValue = _thisAction;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }

    private void DrawExtraParams(SerializedProperty property)
    {
        if (_thisAction == null) return;

        var extraParams = (
            from p in _thisAction.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            where p.HasAttribute<CharacterStateParamAttribute>()
            select p).ToArray();
        foreach (var param in extraParams)
        {
            NextLine();
            var value = param.GetValue(_thisAction);
            var label = new GUIContent(param.Name);
            value = EditorGUIHelper.AutoField(_position, label, value, param.FieldType);
            param.SetValue(_thisAction, value);
        }
    }

    private void NextLine()
    {
        _position.y += _position.height + 2.5f;
        _totalHeight += _position.height + 2.5f;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return _totalHeight;
    }
}
