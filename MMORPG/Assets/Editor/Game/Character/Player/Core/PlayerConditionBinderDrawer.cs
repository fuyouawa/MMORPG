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

    private SerializedProperty _conditionMethodNameProperty;
    private SerializedProperty _conditionMethodObjectProperty;
    private SerializedProperty _attachedObjectProperty;
    private Rect _position;

    private Dictionary<Object, List<MethodInfo>> _conditionMethods = new();

    private int _conditionOffsetSelectedIndex;
    private int _conditionOptionsSelectedIndex;
    private string[] _conditionOptions;

    private Object AttachObjectRef
    {
        get => _attachedObjectProperty.objectReferenceValue;
        set => _attachedObjectProperty.objectReferenceValue = value;
    }

    private string ConditionMethodNameRef
    {
        get => _conditionMethodNameProperty.stringValue;
        set => _conditionMethodNameProperty.stringValue = value;
    }
    private Object ConditionMethodObjectRef
    {
        get => _conditionMethodObjectProperty.objectReferenceValue;
        set => _conditionMethodObjectProperty.objectReferenceValue = value;
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _conditionMethodNameProperty = property.FindPropertyRelative("ConditionMethodName");
        _conditionMethodObjectProperty = property.FindPropertyRelative("ConditionMethodObject");
        _attachedObjectProperty = property.FindPropertyRelative("AttachedObject");

        _position = position;
        _position.height = EditorGUIUtility.singleLineHeight;

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            DrawConditionSelect(property);
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return TotalHeight;
    }


    private void DrawConditionSelect(SerializedProperty property)
    {
        var objectRect = new Rect(_position)
        {
            width = _position.width / 2 - 2.5f
        };

        var conditionsRect = new Rect(_position)
        {
            width = _position.width / 2,
            x = objectRect.x + objectRect.width + 2.5f
        };

        using var check = new EditorGUI.ChangeCheckScope();

        var currentAttachObject = EditorGUI.ObjectField(objectRect, AttachObjectRef, typeof(Object), true);
        if (AttachObjectRef != currentAttachObject)
        {
            AttachedObjectChanged(currentAttachObject);
        }

        RefreshConditionMethodsAndOptions();

        UpdateConditionSelectedIndex();

        _conditionOptionsSelectedIndex = EditorGUI.Popup(conditionsRect, _conditionOptionsSelectedIndex, _conditionOptions);
        if (check.changed)
        {
            CorrectedConditionValueByOptionsSelectedIndex();
            ConditionMethodNameRef = _conditionOptionsSelectedIndex == 0 ?
                string.Empty :
                _conditionMethods[ConditionMethodObjectRef][_conditionOffsetSelectedIndex].Name;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }



    private void RefreshConditionMethodsAndOptions()
    {
        _conditionOptions = new[] { "None Condition" };
        _conditionMethods.Clear();
        if (AttachObjectRef != null)
        {
            if (AttachObjectRef is not Component && AttachObjectRef is not GameObject)
            {
                CombineConditionMethodsAndOptions(AttachObjectRef, GetConditionMethods(AttachObjectRef.GetType()));
                return;
            }

            GameObject gameObject;
            if (AttachObjectRef is Component comp)
                gameObject = comp.gameObject;
            else
                gameObject = (GameObject)AttachObjectRef;

            foreach (var component in gameObject.GetComponents<Component>())
            {
                CombineConditionMethodsAndOptions(component, GetConditionMethods(component.GetType()));
            }
        }
    }

    private void AttachedObjectChanged(Object obj)
    {
        AttachObjectRef = obj;
        ConditionMethodObjectRef = null;
        ConditionMethodNameRef = string.Empty;
    }



    private void CombineConditionMethodsAndOptions(Object obj, List<MethodInfo> methods)
    {
        if (methods.Count == 0)
            return;
        _conditionMethods[obj] = methods;
        var options = methods.Select(x => $"{obj.GetType()}.{x.Name}").ToArray();
        _conditionOptions = _conditionOptions.Combine(options);
    }

    private void CorrectedConditionValueByOptionsSelectedIndex()
    {
        if (_conditionOptionsSelectedIndex == 0)
        {
            ConditionMethodObjectRef = null;
            _conditionOffsetSelectedIndex = 0;
            return;
        }

        var offsetIndex = _conditionOptionsSelectedIndex - 1;
        foreach (var method in _conditionMethods)
        {
            if (offsetIndex < method.Value.Count)
            {
                ConditionMethodObjectRef = method.Key;
                _conditionOffsetSelectedIndex = offsetIndex;
                return;
            }
            offsetIndex -= method.Value.Count;
        }
        _conditionOffsetSelectedIndex = 0;
    }

    private List<MethodInfo> GetConditionMethods(System.Type type)
    {
        return (from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where method.HasAttribute<StateConditionAttribute>()
                select method).ToList();
    }

    private void UpdateConditionSelectedIndex()
    {
        _conditionOptionsSelectedIndex = 0;
        if (ConditionMethodNameRef == string.Empty)
            return;
        int totalIdx = 1;
        foreach (var methods in _conditionMethods)
        {
            if (methods.Key == ConditionMethodObjectRef)
            {
                var idx = methods.Value.FindIndex(x => x.Name == ConditionMethodNameRef);
                if (idx != -1)
                {
                    _conditionOptionsSelectedIndex = totalIdx + idx;
                }
                return;
            }

            totalIdx += methods.Value.Count;
        }
    }
}
