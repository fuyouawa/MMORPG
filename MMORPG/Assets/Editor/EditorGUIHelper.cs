using System;
using QFramework;
using UnityEditor;
using UnityEngine;

public static class EditorGUIHelper
{
    public static object AutoField(Rect position, GUIContent label, object obj, Type type)
    {
        switch (obj)
        {
            case float f:
                return EditorGUI.FloatField(position, label, f);
            case double d:
                return EditorGUI.DoubleField(position, label, d);
            case int i:
                return EditorGUI.IntField(position, label, i);
            case long l:
                return EditorGUI.LongField(position, label, l);
            case string str:
                return EditorGUI.TextField(position, label, str);
            case bool b:
                return EditorGUI.Toggle(position, label, b);
            case Enum e:
                return e.GetType().HasAttribute<FlagsAttribute>() ?
                    EditorGUI.EnumFlagsField(position, label, e) :
                    EditorGUI.EnumPopup(position, label, e);
            default:
                if (type == typeof(UnityEngine.Object))
                    return EditorGUI.ObjectField(position, label, (UnityEngine.Object)obj, type, true);
                throw new NotSupportedException();
        }
    }
}
