using System;
using System.Reflection;
using Sirenix.OdinInspector;

namespace MMORPG.Tool
{
    [Serializable]
    public class ValueSetter
    {
        public enum SupportPropertyType
        {
            Integer,
            FloatingPoint,
            Boolean,
            String,
            UnityObject
        }

        protected SupportPropertyType PropertyType;
        
        [ShowIf("@ValueSettable && PropertyType == SupportPropertyType.Integer")]
        public int IntegralValue;
        [ShowIf("@ValueSettable && PropertyType == SupportPropertyType.FloatingPoint")]
        public float FloatingPointValue;
        [ShowIf("@ValueSettable && PropertyType == SupportPropertyType.Boolean")]
        public bool BooleanValue;
        [ShowIf("@ValueSettable && PropertyType == SupportPropertyType.String")]
        public string StringValue;
        [ShowIf("@ValueSettable && PropertyType == SupportPropertyType.UnityObject")]
        public UnityEngine.Object UnityObjectValue;

        public ValuePicker Picker { get; private set; }

        public void Setup(ValuePicker picker)
        {
            Picker = picker;
        }

        public void Initialize()
        {
            if (Picker != null && Picker.IsValid)
            {
                if (ReflectHelper.IsIntegerType(Picker.TargetValueType))
                    PropertyType = SupportPropertyType.Integer;
                else if (ReflectHelper.IsFloatingPointType(Picker.TargetValueType))
                    PropertyType = SupportPropertyType.FloatingPoint;
                else if (ReflectHelper.IsBooleanType(Picker.TargetValueType))
                    PropertyType = SupportPropertyType.Boolean;
                else if (ReflectHelper.IsStringType(Picker.TargetValueType))
                    PropertyType = SupportPropertyType.String;
                else if (ReflectHelper.IsUnityObject(Picker.TargetValueType))
                    PropertyType = SupportPropertyType.UnityObject;
                else
                    throw new NotSupportedException();
            }
        }

        public virtual object GetValueToSet()
        {
            return PropertyType switch
            {
                SupportPropertyType.Integer => IntegralValue,
                SupportPropertyType.FloatingPoint => FloatingPointValue,
                SupportPropertyType.Boolean => BooleanValue,
                SupportPropertyType.String => StringValue,
                SupportPropertyType.UnityObject => UnityObjectValue,
                _ => throw new ArgumentOutOfRangeException()
            };
        }

#if UNITY_EDITOR
        protected virtual bool ValueSettable => Picker != null && Picker.IsValid;

        [OnInspectorGUI]
        protected virtual void OnInspectorGUI()
        {
            Initialize();
        }
#endif
    }
}
