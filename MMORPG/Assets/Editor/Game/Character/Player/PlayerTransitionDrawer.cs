using System.Linq;
using System.Reflection;
using QFramework;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerTransition))]
public class PlayerTransitionDrawer : PropertyDrawer
{
    public static readonly float LineTotalHeight = EditorGUIUtility.singleLineHeight + 0.5f;
    public static readonly float TotalHeight = LineTotalHeight * 3 + 8f;

    private SerializedProperty _conditionProperty;
    private SerializedProperty _objectProperty;
    private SerializedProperty _trueStateNameProperty;
    private SerializedProperty _falseStateNameProperty;
    private Object _object;

    private Player _target;
    private MethodInfo[] _conditionMethods;
    private int _conditionSelectedIndex;
    private int _trueStatesSelectedIndex;
    private int _falseStatesSelectedIndex;
    private string[] _conditionOptions;
    private string[] _stateOptions;
    private Rect _position;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        _target = (Player)property.serializedObject.targetObject;
        _conditionProperty = property.FindPropertyRelative("Condition");
        _objectProperty = property.FindPropertyRelative("Object");
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

        UpdateConditions();
        _objectProperty.objectReferenceValue = EditorGUI.ObjectField(objectRect, _objectProperty.objectReferenceValue, typeof(Object), true);
        
        UpdateConditionSelectedIndex();

        _conditionSelectedIndex = EditorGUI.Popup(conditionsRect, _conditionSelectedIndex, _conditionOptions);
        if (check.changed)
        {
            _conditionProperty.stringValue = _conditionSelectedIndex == 0 ?
                string.Empty :
                _conditionMethods[_conditionSelectedIndex - 1].Name;
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


    private void UpdateConditions()
    {
        _conditionOptions = new[] { "None Condition" };
        if (_object == _objectProperty.objectReferenceValue)
            return;
        _object = _objectProperty.objectReferenceValue;
        _conditionMethods = null;
        if (_object != null)
        {
            _conditionMethods = (
                from method in _object.GetType().GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                where method.HasAttribute<StateConditionAttribute>()
                select method).ToArray();
            if (_conditionMethods.Length > 0)
            {
                _conditionOptions = _conditionOptions.Combine(_conditionMethods.Select(x => x.Name).ToArray());
            }
        }
    }

    private void UpdateConditionSelectedIndex()
    {
        _conditionSelectedIndex = 0;
        if (_conditionProperty.stringValue == string.Empty)
            return;

        int i = 1;
        foreach (var method in _conditionMethods)
        {
            if (method.Name == _conditionProperty.stringValue)
            {
                _conditionSelectedIndex = i;
                return;
            }
            i++;
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
