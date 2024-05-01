using Malee.List;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CharacterBrain), true)]
public class CharacterBrainEditor : Editor
{
    private SerializedProperty _characterProperty;
    private ReorderableList _statesList;

    private void OnEnable()
    {
        _characterProperty = serializedObject.FindProperty("Character");
        _statesList = new(serializedObject.FindProperty("States"));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.PropertyField(_characterProperty);
        var brain = (CharacterBrain)target;
        if (Application.isPlaying)
        {
            if (brain.CurrentState != null)
                EditorGUILayout.LabelField("CurrentState", brain.CurrentState.StateName);
        }
        _statesList.DoLayoutList();

        serializedObject.ApplyModifiedProperties();
    }
}
