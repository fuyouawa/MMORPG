using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

[CustomPropertyDrawer(typeof(DropdownAttribute))]
public class DropdownAttributeDrawer : PropertyDrawer
{
    private DropdownAttribute _dropdownAttribute;
    private string[] _dropdownValues;
    private int _selectedDropdownValueIndex = -1;
    private Type _propertyType;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_dropdownAttribute == null)
        {
            _dropdownAttribute = (DropdownAttribute)attribute;
            if (_dropdownAttribute.DropdownValues == null || _dropdownAttribute.DropdownValues.Length == 0)
            {
                return;
            }

            _propertyType = _dropdownAttribute.DropdownValues[0].GetType();

            _dropdownValues = new string[_dropdownAttribute.DropdownValues.Length];
            for (int i = 0; i < _dropdownAttribute.DropdownValues.Length; i++)
            {
                _dropdownValues[i] = _dropdownAttribute.DropdownValues[i].ToString();
            }

            bool found = false;
            for (var i = 0; i < _dropdownValues.Length; i++)
            {
                if ((_propertyType == typeof(string)) && property.stringValue == _dropdownValues[i])
                {
                    _selectedDropdownValueIndex = i;
                    found = true;
                    break;
                }
                if ((_propertyType == typeof(int)) && property.intValue == Convert.ToInt32(_dropdownValues[i]))
                {
                    _selectedDropdownValueIndex = i;
                    found = true;
                    break;
                }
                if ((_propertyType == typeof(float)) && Mathf.Approximately(property.floatValue, Convert.ToSingle(_dropdownValues[i])))
                {
                    _selectedDropdownValueIndex = i;
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                _selectedDropdownValueIndex = 0;
            }
        }

        if ((_dropdownValues == null) || (_dropdownValues.Length == 0) || (_selectedDropdownValueIndex < 0))
        {
            EditorGUI.PropertyField(position, property, label);
            return;
        }

        EditorGUI.BeginChangeCheck();
        _selectedDropdownValueIndex = EditorGUI.Popup(position, label.text, _selectedDropdownValueIndex, _dropdownValues);
        if (EditorGUI.EndChangeCheck())
        {
            if (_propertyType == typeof(string))
            {
                property.stringValue = _dropdownValues[_selectedDropdownValueIndex];
            }
            else if (_propertyType == typeof(int))
            {
                property.intValue = Convert.ToInt32(_dropdownValues[_selectedDropdownValueIndex]);
            }
            else if (_propertyType == typeof(float))
            {
                property.floatValue = Convert.ToSingle(_dropdownValues[_selectedDropdownValueIndex]);
            }
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}
