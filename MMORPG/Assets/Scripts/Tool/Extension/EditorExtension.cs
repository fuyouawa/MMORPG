using System;
using UnityEditor;
using Object = UnityEngine.Object;

#if UNITY_EDITOR

public static class EditorExtension
{
    public static object GetValue(this SerializedProperty property, Type type)
    {
        if (type == typeof(string))
            return property.stringValue;
        if (type == typeof(bool))
            return property.boolValue;
        if (type == typeof(float))
            return property.floatValue;
        if (type == typeof(int))
            return property.intValue;
        if (type == typeof(uint))
            return property.uintValue;
        if (type == typeof(ulong))
            return property.ulongValue;
        if (type == typeof(double))
            return property.doubleValue;
        if (type.IsValueType)
            throw new NotSupportedException();
        return property.objectReferenceValue;
    }

    public static void SetValue(this SerializedProperty property, object val)
    {
        switch (val)
        {
            case string str:
                property.stringValue = str;
                break;
            case bool b:
                property.boolValue = b;
                break;
            case int i:
                property.intValue = i;
                break;
            case long l:
                property.longValue = l;
                break;
            case float f:
                property.floatValue = f;
                break;
            case double d:
                property.doubleValue = d;
                break;
            case uint ui:
                property.uintValue = ui;
                break;
            case ulong ul:
                property.ulongValue = ul;
                break;
            case Object obj:
                property.objectReferenceValue = obj;
                break;
            default:
                if (val == null)
                {
                    property.objectReferenceValue = null;
                    return;
                }
                if (val.GetType().IsValueType)
                    throw new NotSupportedException();
                break;
        }
    }
}
#endif

