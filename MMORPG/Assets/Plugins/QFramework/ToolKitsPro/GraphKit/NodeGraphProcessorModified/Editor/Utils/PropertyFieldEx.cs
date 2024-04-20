using System;
using UnityEditor;

namespace GraphProcessor
{
    public static class PropertyFieldEx
    {
        static void RegisterUpdateValueChanged<T>(this SerializedProperty self, Func<SerializedProperty, T> valueGetter,
            Action<SerializedProperty> callback)
        {
            var previousValue = valueGetter(self);
            EditorApplication.CallbackFunction callbackFunction = null;
            callbackFunction = () =>
            {
                if (self != null)
                {
                    if (!previousValue.Equals(valueGetter(self)))
                    {
                        callback(self);
                    }
                    else
                    {
                        EditorApplication.update -= callbackFunction;
                    }
                }

                EditorApplication.update += callbackFunction;
            };
        }

        public static void RegisterValueChangedCallback2019(this SerializedProperty self,
            Action<SerializedProperty> callback)
        {
            var property = self;

            if (property == null) return;
            
            switch (property.propertyType)
            {
                case SerializedPropertyType.Boolean:
                    RegisterUpdateValueChanged(property, p => p.boolValue, callback);
                    break;
                case SerializedPropertyType.Integer:
                    RegisterUpdateValueChanged(property, p => p.intValue, callback);
                    break;
                case SerializedPropertyType.Float:
                    RegisterUpdateValueChanged(property, p => p.floatValue, callback);
                    break;
                case SerializedPropertyType.String:
                    RegisterUpdateValueChanged(property, p => p.stringValue, callback);
                    break;
            }

           
        }
    }
}