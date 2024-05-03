using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(PlayerAction))]
public class PlayerActionDrawer : PropertyDrawer
{
    public static readonly float LineTotalHeight = EditorGUIUtility.singleLineHeight + 0.5f;
    public static readonly float TotalHeight = LineTotalHeight * 2f + 8f;

    private PlayerAction _currentAction;
    private PlayerAction[] _actions;
    private int _selectedIndex = 0;
    private string[] _options;
    private Rect _position;
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        using (new EditorGUI.PropertyScope(position, label, property))
        {
            _position = position;
            _position.y += 5f;
            _position.x += 10f;
            _position.width -= 10f;
            _position.height = EditorGUIUtility.singleLineHeight;

            RefreshValue(property);
            DrawDropdownSelections(property);
            NextLine();
            EditorGUI.PropertyField(_position, property, label);
        }
    }

    private void DrawDropdownSelections(SerializedProperty property)
    {
        EditorGUI.BeginChangeCheck();
        _selectedIndex = EditorGUI.Popup(_position, _selectedIndex, _options);
        if (EditorGUI.EndChangeCheck())
        {
            _currentAction = (_selectedIndex == 0) ? null : _actions[_selectedIndex - 1];
            property.objectReferenceValue = _currentAction;
            property.serializedObject.ApplyModifiedProperties();
            EditorUtility.SetDirty(property.serializedObject.targetObject);
        }
    }

    private void RefreshValue(SerializedProperty property)
    {
        _currentAction = property.objectReferenceValue as PlayerAction;
        _actions = (property.serializedObject.targetObject as PlayerBrain).GetAttachActions();
        _options = new string[_actions.Length + 1];
        _options[0] = "None Action";
        _selectedIndex = 0;
        int i = 1;
        foreach (var action in _actions)
        {
            _options[i] = $"{i} - {action.GetType()}";
            if (action == _currentAction)
                _selectedIndex = i;
            i++;
        }
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return TotalHeight;
    }

    private void NextLine()
    {
        _position.y += LineTotalHeight;
    }
}
