using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerTransition))]
public class PlayerTransitionDrawer : PropertyDrawer
{
    public static readonly float LineTotalHeight = EditorGUIUtility.singleLineHeight + 0.5f;

    private SerializedProperty _conditionBindersProperty;
    private SerializedProperty _trueStateNameProperty;
    private SerializedProperty _falseStateNameProperty;

    private PlayerBrain _target;

    private int _trueStatesSelectedIndex;
    private int _falseStatesSelectedIndex;
    private string[] _stateOptions;

    private Rect _position;
    private EditorGUIHelper.ListGroup _listGroup;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _listGroup = new();
        _target = (PlayerBrain)property.serializedObject.targetObject;
        _conditionBindersProperty = property.FindPropertyRelative("ConditionBinders");
        _trueStateNameProperty = property.FindPropertyRelative("TrueStateName");
        _falseStateNameProperty = property.FindPropertyRelative("FalseStateName");

        _position = position;
        _position.y += 5;
        _position.height = EditorGUIUtility.singleLineHeight;

        using (new EditorGUI.PropertyScope(position, label, property))
        {
            DrawExpandTitle(property);
            if (property.isExpanded)
            {
                _listGroup.NextLine(ref _position);

                EditorGUI.PropertyField(_position, _conditionBindersProperty, new GUIContent("Conditions"));

                _listGroup.NextLine(_conditionBindersProperty, ref _position);

                DrawStateSelect(property, _trueStateNameProperty, "TrueState", out _trueStatesSelectedIndex);

                _listGroup.NextLine(ref _position);

                DrawStateSelect(property, _falseStateNameProperty, "FalseState", out _falseStatesSelectedIndex);
            }
        }
    }

    private void DrawExpandTitle(SerializedProperty property)
    {
        var style = new GUIStyle();
        style.normal.background = MakeBackgroundTexture(1, 1, property.isExpanded ? Color.yellow : Color.yellow * 0.8f);
        style.margin = new RectOffset(4, 4, 2, 2);
        style.alignment = TextAnchor.MiddleCenter;

        // var conditionMethodObject =
        //     _conditionBinderProperty.FindPropertyRelative("ConditionMethodObject").objectReferenceValue;
        // var conditionMethodName =
        //     _conditionBinderProperty.FindPropertyRelative("ConditionMethodName").stringValue;
        // var title = conditionMethodObject != null
        //     ? $"{conditionMethodObject.GetType()} - {conditionMethodName}"
        //     : "NONE";
        var title = _conditionBindersProperty.arraySize == 0 ? "None Title" : "";
        for (int i = 0; i < _conditionBindersProperty.arraySize; i++)
        {
            var conditionBinder = _conditionBindersProperty.GetArrayElementAtIndex(i);
            var methodObject = conditionBinder.FindPropertyRelative("MethodObject").objectReferenceValue;
            var methodName = conditionBinder.FindPropertyRelative("MethodName").stringValue;
            title += methodName;
            if (i != _conditionBindersProperty.arraySize - 1)
            {
                title += " && ";
            }
        }
        if (GUI.Button(_position, title, style))
        {
            property.isExpanded = !property.isExpanded;
        }
        _position.y += 5f;
    }

    private Texture2D MakeBackgroundTexture(int width, int height, Color color)
    {
        var pixels = new Color[width * height];

        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = color;
        }

        var backgroundTexture = new Texture2D(width, height);

        backgroundTexture.SetPixels(pixels);
        backgroundTexture.Apply();

        return backgroundTexture;
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return GetHeight(property);
    }

    private static float GetHeight(SerializedProperty property)
    {
        if (!property.isExpanded)
            return LineTotalHeight + 5f;
        EditorGUIHelper.ListGroup listGroup = new();
        listGroup.NextLines(3);
        listGroup.NextLine(property.FindPropertyRelative("ConditionBinders"));
        return listGroup.Height + 9f;
    }


    private void DrawStateSelect(SerializedProperty property, SerializedProperty stateNameProperty, string displayName, out int statesSelectedIndex)
    {
        UpdateStates();
        UpdateStateSelectedIndex(stateNameProperty, out statesSelectedIndex);

        // var labelRect = new Rect(_position)
        // {
        //     width = _position.width / 2 - 2.5f,
        //     y = _position.y + _listGroup.Height
        // };
        var popupRect = new Rect(_position)
        {
            // width = _position.width / 2,
            // x = labelRect.x + labelRect.width + 2.5f
        };

        using var check = new EditorGUI.ChangeCheckScope();

        // EditorGUI.LabelField(labelRect, displayName);
        statesSelectedIndex = EditorGUI.Popup(popupRect, displayName, statesSelectedIndex, _stateOptions);
        if (check.changed)
        {
            var state = statesSelectedIndex == 0
                ? null
                : _target.States[statesSelectedIndex - 1];
            stateNameProperty.stringValue = state == null ? string.Empty : state.Name;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
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
}
