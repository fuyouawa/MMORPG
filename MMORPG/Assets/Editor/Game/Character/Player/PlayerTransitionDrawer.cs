using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using QFramework;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

[CustomPropertyDrawer(typeof(PlayerTransition))]
public class PlayerTransitionDrawer : PropertyDrawer
{
    public static readonly float LineTotalHeight = EditorGUIUtility.singleLineHeight + 0.5f;
    public static readonly float TotalHeight = LineTotalHeight * 3 + 8f;

    private SerializedProperty _conditionMethodNameProperty;
    private SerializedProperty _conditionMethodObjectProperty;
    private SerializedProperty _attachedObjectProperty;
    private SerializedProperty _trueStateNameProperty;
    private SerializedProperty _falseStateNameProperty;

    private Object AttachObjectRef
    {
        get => _attachedObjectProperty.objectReferenceValue;
        set => _attachedObjectProperty.objectReferenceValue = value;
    }

    private string CurrentConditionMethodNameRef
    {
        get => _conditionMethodNameProperty.stringValue;
        set => _conditionMethodNameProperty.stringValue = value;
    }
    private Object ConditionMethodObjectRef
    {
        get => _conditionMethodObjectProperty.objectReferenceValue;
        set => _conditionMethodObjectProperty.objectReferenceValue = value;
    }

    private Player _target;
    private Dictionary<Object, List<MethodInfo>> _conditionMethods = new();

    private int _conditionOptionsSelectedIndex;
    private int _conditionOffsetSelectedIndex;
    private int _trueStatesSelectedIndex;
    private int _falseStatesSelectedIndex;
    private string[] _conditionOptions;
    private string[] _stateOptions;
    private Rect _position;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _target = (Player)property.serializedObject.targetObject;
        _conditionMethodNameProperty = property.FindPropertyRelative("ConditionMethodName");
        _conditionMethodObjectProperty = property.FindPropertyRelative("ConditionMethodObject");
        _attachedObjectProperty = property.FindPropertyRelative("AttachedObject");
        _trueStateNameProperty = property.FindPropertyRelative("TrueStateName");
        _falseStateNameProperty = property.FindPropertyRelative("FalseStateName");

        _position = position;
        _position.y += 5f;
        _position.height = EditorGUIUtility.singleLineHeight;
        
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            DrawIcon();
            DrawConditionSelect(property);
            NextLine();
            UpdateStates();
            DrawStateSelect(property, _trueStateNameProperty, "TrueState", out _trueStatesSelectedIndex);
            NextLine();
            DrawStateSelect(property, _falseStateNameProperty, "FalseState", out _falseStatesSelectedIndex);
        }
    }

    private void DrawIcon()
    {
        var iconRect = new Rect(_position)
        {
            x = _position.x - 25f,
            y = _position.y + LineTotalHeight
        };
        EditorGUI.LabelField(iconRect, EditorGUIUtility.IconContent("d_AnimatorStateTransition Icon"));
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
            CurrentConditionMethodNameRef = _conditionOptionsSelectedIndex == 0 ?
                string.Empty :
                _conditionMethods[ConditionMethodObjectRef][_conditionOffsetSelectedIndex].Name;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }


    private void DrawStateSelect(SerializedProperty property, SerializedProperty stateNameProperty, string displayName, out int statesSelectedIndex)
    {
        UpdateStateSelectedIndex(stateNameProperty, out statesSelectedIndex);

        var labelRect = new Rect(_position)
        {
            width = _position.width / 2 - 2.5f
        };
        var popupRect = new Rect(_position)
        {
            width = _position.width / 2,
            x = labelRect.x + labelRect.width + 2.5f
        };

        using var check = new EditorGUI.ChangeCheckScope();

        EditorGUI.LabelField(labelRect, displayName);
        statesSelectedIndex = EditorGUI.Popup(popupRect, statesSelectedIndex, _stateOptions);
        if (check.changed)
        {
            var state = statesSelectedIndex == 0 ?
                null :
                _target.States[statesSelectedIndex - 1];
            stateNameProperty.stringValue = state == null ? string.Empty : state.Name;
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
        CurrentConditionMethodNameRef = string.Empty;
    }

    private void CombineConditionMethodsAndOptions(Object obj, List<MethodInfo> methods)
    {
        if (methods.Count == 0)
            return;
        _conditionMethods[obj] = methods;
        var options = _conditionMethods
            .SelectMany(pair => pair.Value)
            .Select(x => $"{obj.GetType()}.{x.Name}").ToArray();
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

    private List<MethodInfo> GetConditionMethods(Type type)
    {
        return (from method in type.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where method.HasAttribute<StateConditionAttribute>()
                select method).ToList();
    }

    private void UpdateConditionSelectedIndex()
    {
        _conditionOptionsSelectedIndex = 0;
        if (CurrentConditionMethodNameRef == string.Empty)
            return;
        int totalIdx = 1;
        foreach (var methods in _conditionMethods)
        {
            if (methods.Key == ConditionMethodObjectRef)
            {
                var idx = methods.Value.FindIndex(x => x.Name == CurrentConditionMethodNameRef);
                if (idx != -1)
                {
                    _conditionOptionsSelectedIndex = totalIdx + idx;
                }
                return;
            }

            totalIdx += methods.Value.Count;
        }
    }

    private void UpdateStateSelectedIndex(SerializedProperty stateNameProperty, out int selectIndex)
    {
        selectIndex = 0;
        if (stateNameProperty.stringValue == string.Empty)
            return;

        int i = 1;
        foreach (var state in _target.States)
        {
            if (state.Name == stateNameProperty.stringValue)
            {
                selectIndex = i;
                return;
            }
            i++;
        }
    }

    private void UpdateStates()
    {
        _stateOptions = new[] { "None State" };
        if (_target.States.Count > 0)
        {
            _stateOptions = _stateOptions.Combine(
                _target.States.Select((x, i) => $"{i} - {x.Name}"
                ).ToArray());
        }
    }

    private void NextLine()
    {
        _position.y += LineTotalHeight;
    }
}
