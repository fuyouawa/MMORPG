using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Reflection;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(DropdownRuntimeAttribute))]
public class DropdownRuntimeAttributeDrawer : PropertyDrawer
{
    private DropdownRuntimeAttribute _dropdownAttribute;
    private MethodInfo _contentGetterMethod;
    private Func<IEnumerable<object>> _contentGetter;

    private List<object> _content;
    private string[] _options;
    private Type _propertyType;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (_dropdownAttribute == null)
        {
            Initialize(property);
        }
        UpdateValue(property);

        var labelText = _dropdownAttribute.Label == string.Empty ? label.text : _dropdownAttribute.Label;

        if (_content.Count == 0)
        {
            EditorGUI.Popup(position, labelText, 0, new[] { "NONE" });
            return;
        }

        var curValue = property.GetValue(_propertyType);
        var selectedIndex = _content.FindIndex(o =>
        {
            if (o == null)
                return curValue == null;
            return o.Equals(curValue);
        });
        if (selectedIndex == -1)
            selectedIndex = 0;

        EditorGUI.BeginChangeCheck();
        selectedIndex = EditorGUI.Popup(position, labelText, selectedIndex, _options);
        if (EditorGUI.EndChangeCheck())
        {
            curValue = _content[selectedIndex];
            property.SetValue(curValue);
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }

    private string FormatName(object val, int i)
    {
        return $"{i} - {Convert.ToString(val)}";
    }

    private void Initialize(SerializedProperty property)
    {
        _dropdownAttribute = (DropdownRuntimeAttribute)attribute;

        _contentGetterMethod =
            property.serializedObject.targetObject.GetType().GetMethod(
                _dropdownAttribute.ContentGetter,
                BindingFlags.Instance | BindingFlags.Public);
        if (_contentGetterMethod == null)
            throw new Exception($"在{property.serializedObject.targetObject}中没有函数{_dropdownAttribute.ContentGetter}");

        _contentGetter = () =>
        {
            var ret = _contentGetterMethod.Invoke(property.serializedObject.targetObject, null);
            if (ret is not IEnumerable enumerable)
                throw new InvalidCastException("DropdownRuntime中的ContentGetter必须是一个返回(继承了)IEnumerable的函数!");
            return enumerable.Cast<object>();
        };
    }


    private void UpdateValue(SerializedProperty property)
    {
        if (_contentGetterMethod == null)
            throw new Exception($"在{property.serializedObject.targetObject}中没有函数{_dropdownAttribute.ContentGetter}");

        _content = _contentGetter().ToList();

        if (_content.Count == 0)
        {
            return;
        }
        _propertyType = _content[0].GetType();

        if (!_dropdownAttribute.Flags.HasFlag(DropdownRuntimeFlags.DontAddNoneInContentFirst))
            _content.Insert(0, _propertyType.DefaultInstance());

        if (!_dropdownAttribute.Flags.HasFlag(DropdownRuntimeFlags.DontAddNoneInContentFirst))
        {
            var tmp = _content.Skip(1).Select(FormatName).ToList();
            tmp.Insert(0, "NONE");
            _options = tmp.ToArray();
        }
        else
            _options = _content.Select(FormatName).ToArray();
    }
}
