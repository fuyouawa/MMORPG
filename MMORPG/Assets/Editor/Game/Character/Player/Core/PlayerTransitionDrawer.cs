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
    public static readonly float TotalHeight = LineTotalHeight * 4 + 9f;

    private SerializedProperty _conditionMethodNameProperty;
    private SerializedProperty _conditionMethodObjectProperty;
    private SerializedProperty _attachedObjectProperty;
    private SerializedProperty _trueStateNameProperty;
    private SerializedProperty _falseStateNameProperty;
    private SerializedProperty _isExpandingProperty;

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
    private bool IsExpandingRef
    {
        get => _isExpandingProperty.boolValue;
        set => _isExpandingProperty.boolValue = value;
    }

    private PlayerBrain _target;
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
        _target = (PlayerBrain)property.serializedObject.targetObject;
        _conditionMethodNameProperty = property.FindPropertyRelative("ConditionMethodName");
        _conditionMethodObjectProperty = property.FindPropertyRelative("ConditionMethodObject");
        _attachedObjectProperty = property.FindPropertyRelative("AttachedObject");
        _trueStateNameProperty = property.FindPropertyRelative("TrueStateName");
        _falseStateNameProperty = property.FindPropertyRelative("FalseStateName");
        _isExpandingProperty = property.FindPropertyRelative("IsExpanding");

        _position = position;
        _position.y += 5;
        _position.height = EditorGUIUtility.singleLineHeight;
        
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            DrawExpandTitle();
            if (IsExpandingRef)
            {
                NextLine();
                DrawConditionSelect(property);
                NextLine();
                UpdateStates();
                DrawStateSelect(property, _trueStateNameProperty, "TrueState", out _trueStatesSelectedIndex);
                NextLine();
                DrawStateSelect(property, _falseStateNameProperty, "FalseState", out _falseStatesSelectedIndex);
            }
        }
    }

    private void DrawExpandTitle()
    {
        var style = new GUIStyle();
        style.normal.background = MakeBackgroundTexture(1, 1, IsExpandingRef ? Color.yellow : Color.yellow * 0.8f);
        style.margin = new RectOffset(4, 4, 2, 2);
        style.alignment = TextAnchor.MiddleCenter;

        if (GUI.Button(_position, $"{ConditionMethodObjectRef.GetType()} - {ConditionMethodNameRef}", style))
        {
            IsExpandingRef = !IsExpandingRef;
        }
        _position.y += 5f;
    }

    private Texture2D MakeBackgroundTexture(int width, int height, Color color)
    {
        Color[] pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        Texture2D backgroundTexture = new Texture2D(width, height);

        backgroundTexture.SetPixels(pixels);
        backgroundTexture.Apply();

        return backgroundTexture;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        _isExpandingProperty = property.FindPropertyRelative("IsExpanding");
        return IsExpandingRef ? TotalHeight : LineTotalHeight + 5f;
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

    private List<MethodInfo> GetConditionMethods(Type type)
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
