using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using QFramework;
using Sirenix.OdinInspector;
using UnityEngine;

namespace MMORPG.Tool
{
    [Flags]
    public enum ValuePickerMemberTypeMasks
    {
        None = 0,
        Field = 1,
        ReadableProperty = 1 << 1,
        WriteableProperty = 1 << 2,
        Method = 1 << 3,
        Property = ReadableProperty | WriteableProperty,
        Variable = Field | Property,
        All = Field | Property | Method
    }

    [Serializable]
    public class ValuePicker
    {
        [Required]
        public GameObject TargetObject;
        [Required]
        [ValueDropdown("GetTargetComponentsDropdown")]
        public Component TargetComponent;
        [Required]
        [LabelText("Target Member")]
        [ValueDropdown("GetComponentMembersDropdown")]
        public string TargetMemberName;

        public MemberInfo TargetMember { get; private set; }
        public Type ValueType { get; private set; }

        public bool CanSetValue { get; private set; }
        public bool IsValid { get; private set; }

        private Func<object> _getter;
        private Action<object> _setter;

        public ValuePickerMemberTypeMasks MemberTypeMasks { get; set; } = ValuePickerMemberTypeMasks.Variable;
        public BindingFlags MemberBindingFlags { get; set; } = BindingFlags.Instance | BindingFlags.Public;

        public void Initialize()
        {
            if (TargetObject && TargetComponent && TargetMemberName.IsNotNullAndEmpty())
            {
                IsValid = false;
                return;
            }

            IsValid = true;
            var field = TargetComponent.GetType().GetField(TargetMemberName, MemberBindingFlags);
            if (field != null)
            {
                TargetMember = field;
                CanSetValue = true;
                ValueType = field.FieldType;
                _getter = () => field.GetValue(TargetComponent);
                _setter = obj => field.SetValue(TargetComponent, obj);
                return;
            }
            var property = TargetComponent.GetType().GetProperty(TargetMemberName, MemberBindingFlags);
            if (property != null)
            {
                TargetMember = property;
                CanSetValue = property.CanWrite;
                ValueType = property.PropertyType;
                _getter = () => property.GetValue(TargetComponent);
                _setter = obj => property.SetValue(TargetComponent, obj);
                return;
            }
            var method = TargetComponent.GetType().GetMethod(TargetMemberName, MemberBindingFlags);
            if (method != null)
            {
                TargetMember = method;
                CanSetValue = false;
                ValueType = method.ReturnType;
                _getter = () => method.Invoke(TargetComponent, null);
                return;
            }
            IsValid = false;
        }

        public object GetValue()
        {
            Debug.Assert(IsValid);
            return _getter();
        }

        public void SetValue(object value)
        {
            if (!CanSetValue)
                throw new Exception("TargetMember is get only!");
            _setter(value);
        }

#if UNITY_EDITOR
        private IEnumerable GetTargetComponentsDropdown()
        {
            var total = new ValueDropdownList<Component>() { { "None", null } };
            if (TargetObject == null)
                return total;
            total.AddRange(TargetObject.GetComponents<Component>()
                .Select(x => new ValueDropdownItem<Component>(x.GetType().Name, x)));
            return total;
        }

        private IEnumerable GetComponentMembersDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (TargetComponent == null)
                return total;
            if ((MemberTypeMasks & ValuePickerMemberTypeMasks.Field) != 0)
            {
                total.AddRange(TargetComponent.GetType().GetFields(MemberBindingFlags).Select(x =>
                    new ValueDropdownItem<string>($"Field / {x.Name} [{x.FieldType.Name}]", x.Name)));
            }
            if ((MemberTypeMasks & ValuePickerMemberTypeMasks.Property) != 0)
            {
                bool Filter(PropertyInfo property)
                {
                    var readableNeeded = (MemberTypeMasks & ValuePickerMemberTypeMasks.ReadableProperty) != 0;
                    var writeableNeeded = (MemberTypeMasks & ValuePickerMemberTypeMasks.WriteableProperty) != 0;
                    if (readableNeeded)
                    {
                        if (writeableNeeded)
                            return property.CanRead && property.CanWrite;
                        else
                            return property.CanRead && !property.CanWrite;
                    }
                    else
                    {
                        return !property.CanRead && property.CanWrite;
                    }
                }

                total.AddRange(TargetComponent.GetType().GetProperties(MemberBindingFlags)
                    .Where(Filter)
                    .Select(x =>
                        new ValueDropdownItem<string>($"Property / {x.Name} [{x.PropertyType.Name}]", x.Name))
                );
            }
            if ((MemberTypeMasks & ValuePickerMemberTypeMasks.Method) != 0)
            {
                total.AddRange(TargetComponent.GetType().GetMethods(MemberBindingFlags)
                    .Where(x => x.GetParameters().IsNullOrEmpty())
                    .Select(x =>
                        new ValueDropdownItem<string>($"Method / {x.Name} [{x.ReturnType.Name}]", x.Name))
                );
            }
            return total;
        }
#endif
    }
}
