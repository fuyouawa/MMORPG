using System;
using Sirenix.OdinInspector;

namespace MMORPG.Tool
{
    [Serializable]
    public class VisualObject
    {

        [LabelText("@Label")]
        [ShowIf("@_propertyType == SupportPropertyType.Integer")]
        public int IntegralValue;
        [LabelText("@Label")]
        [ShowIf("@_propertyType == SupportPropertyType.FloatingPoint")]
        public float FloatingPointValue;
        [LabelText("@Label")]
        [ShowIf("@_propertyType == SupportPropertyType.Boolean")]
        public bool BooleanValue;
        [LabelText("@Label")]
        [ShowIf("@_propertyType == SupportPropertyType.String")]
        public string StringValue;
        [LabelText("@Label")]
        [ShowIf("@_propertyType == SupportPropertyType.UnityObject")]
        public UnityEngine.Object UnityObjectValue;

        private enum SupportPropertyType
        {
            None,
            Integer,
            FloatingPoint,
            Boolean,
            String,
            Enum,
            UnityObject
        }

        private SupportPropertyType _propertyType;

        public string Label { get; private set; }
        public Type ObjectType;

        public void Setup(Type type, string label = null)
        {
            if (ReflectHelper.IsIntegerType(type))
                _propertyType = SupportPropertyType.Integer;
            else if (ReflectHelper.IsFloatingPointType(type))
                _propertyType = SupportPropertyType.FloatingPoint;
            else if (ReflectHelper.IsBooleanType(type))
                _propertyType = SupportPropertyType.Boolean;
            else if (ReflectHelper.IsStringType(type))
                _propertyType = SupportPropertyType.String;
            else if (ReflectHelper.IsUnityObject(type))
                _propertyType = SupportPropertyType.UnityObject;
            else if (type.IsEnum)
                _propertyType = SupportPropertyType.Enum;
            else
                _propertyType = SupportPropertyType.None;

            Label = label ?? GetDefaultLabel();
            ObjectType = type;
        }

        public object GetRawValue()
        {
            return _propertyType switch
            {
                SupportPropertyType.Integer => IntegralValue,
                SupportPropertyType.FloatingPoint => FloatingPointValue,
                SupportPropertyType.Boolean => BooleanValue,
                SupportPropertyType.String => StringValue,
                SupportPropertyType.Enum => throw new NotImplementedException(),
                SupportPropertyType.UnityObject => UnityObjectValue,
                _ => null
            };
        }

        private string GetDefaultLabel()
        {
            switch (_propertyType)
            {
                case SupportPropertyType.Integer:
                    return "IntegralValue";
                case SupportPropertyType.FloatingPoint:
                    return "FloatingPointValue";
                case SupportPropertyType.Boolean:
                    return "BooleanValue";
                case SupportPropertyType.String:
                    return "StringValue";
                case SupportPropertyType.Enum:
                    return "EnumValue";
                case SupportPropertyType.UnityObject:
                    return "UnityObjectValue";
                default:
                    return string.Empty;
            }
        }
    }
}
