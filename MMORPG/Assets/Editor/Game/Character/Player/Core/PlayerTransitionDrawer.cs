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

    private SerializedProperty _conditionBinderProperty;
    private SerializedProperty _trueStateNameProperty;
    private SerializedProperty _falseStateNameProperty;

    private PlayerBrain _target;

    private int _trueStatesSelectedIndex;
    private int _falseStatesSelectedIndex;
    private string[] _stateOptions;
    private Rect _position;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _target = (PlayerBrain)property.serializedObject.targetObject;
        _conditionBinderProperty = property.FindPropertyRelative("ConditionBinder");
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
                NextLine();
                EditorGUI.PropertyField(_position, _conditionBinderProperty);
                NextLine();
                UpdateStates();
                DrawStateSelect(property, _trueStateNameProperty, "TrueState", out _trueStatesSelectedIndex);
                NextLine();
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

        var conditionMethodObject =
            _conditionBinderProperty.FindPropertyRelative("ConditionMethodObject").objectReferenceValue;
        var conditionMethodName =
            _conditionBinderProperty.FindPropertyRelative("ConditionMethodName").stringValue;
        var title = conditionMethodObject != null
            ? $"{conditionMethodObject.GetType()} - {conditionMethodName}"
            : "NONE";
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
        return property.isExpanded ? TotalHeight : LineTotalHeight + 5f;
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

    private void NextLine()
    {
        _position.y += LineTotalHeight;
    }
}
