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
        Default = 0,
        Field = 1,
        ReadOnlyProperty = 1 << 1,
        WritOnlyProperty = 1 << 2,
        ReadAndWriteProperty = 1 << 3,
        Method = 1 << 4,
        AllProperty = ReadOnlyProperty | WritOnlyProperty | ReadAndWriteProperty,
        WriteableVariable = Field | WritOnlyProperty | ReadAndWriteProperty,
        Variable = Field | AllProperty,
        All = Variable | Method
    }

    [Flags]
    public enum ValuePickerValueTypeFilters
    {
        Default = 0,
        Primitive = 1,
        UnityObject = 1 << 1,
        CSharpObject = 1 << 2,
        Graphicizable = Primitive | UnityObject,
        All = Primitive | UnityObject | CSharpObject
    }

    [Serializable]
    public class ValuePicker
    {
        [Required]
        [OnValueChanged("OnValueChanged")]
        public GameObject TargetObject;

        [Required]
        [ValueDropdown("GetTargetComponentsDropdown")]
        [OnValueChanged("OnValueChanged")]
        public Component TargetComponent;

        [OnValueChanged("OnValueChanged")]
        public bool IncludePrivate = false;

        [Required]
        [LabelText("Target Member")]
        [ValueDropdown("GetComponentMemberNamesDropdown")]
        [OnValueChanged("OnValueChanged")]
        public string TargetMemberName;

        public MemberInfo TargetMember { get; private set; }
        public Type TargetValueType { get; private set; }

        public bool CanSetValue { get; private set; }
        public bool IsValid { get; private set; }

        private Func<object> _getter;
        private Action<object> _setter;

        public ValuePickerValueTypeFilters ValueTypeFilters { get; set; }
        public ValuePickerMemberTypeMasks MemberTypeMasks { get; set; }
        public BindingFlags MemberBindingFlags { get; set; }

        public ValuePicker(
            bool includePrivate = false,
            ValuePickerMemberTypeMasks memberTypeMasks = ValuePickerMemberTypeMasks.Default,
            ValuePickerValueTypeFilters valueTypeFilters = ValuePickerValueTypeFilters.Default,
            BindingFlags memberBindingFlags = BindingFlags.Instance | BindingFlags.Public)
        {
            IncludePrivate = includePrivate;
            MemberTypeMasks = memberTypeMasks;
            ValueTypeFilters = valueTypeFilters;
            MemberBindingFlags = memberBindingFlags;
            if (IncludePrivate)
                MemberBindingFlags |= BindingFlags.NonPublic;
        }

        public virtual void InitializeAndCheckValid()
        {
            IsValid = false;
            if (!TargetObject || !TargetComponent || TargetMemberName.IsNullOrEmpty())
            {
                return;
            }

            var splits = TargetMemberName.Split('/');
            if (splits.Length != 2)
            {
                return;
            }
            var type = splits[0];
            var memberName = splits[1];

            switch (type)
            {
                case "Field":
                {
                    var field = TargetComponent.GetType().GetField(memberName, MemberBindingFlags);
                    if (field != null)
                    {
                        TargetMember = field;
                        CanSetValue = field.IsPublic;
                        TargetValueType = field.FieldType;
                        _getter = () => field.GetValue(TargetComponent);
                        _setter = obj => field.SetValue(TargetComponent, obj);
                        IsValid = true;
                        break;
                    }
                    return;
                    }
                case "Property":
                {
                    var property = TargetComponent.GetType().GetProperty(memberName, MemberBindingFlags);
                    if (property != null)
                    {
                        TargetMember = property;
                        CanSetValue = property.CanWrite;
                        TargetValueType = property.PropertyType;
                        _getter = () => property.GetValue(TargetComponent);
                        _setter = obj => property.SetValue(TargetComponent, obj);
                        IsValid = true;
                        break;
                    }
                    return;
                    }
                case "Method":
                {
                    var method = TargetComponent.GetType().GetMethod(memberName, MemberBindingFlags);
                    if (method != null)
                    {
                        TargetMember = method;
                        CanSetValue = false;
                        TargetValueType = method.ReturnType;
                        _getter = () => method.Invoke(TargetComponent, null);
                        IsValid = true;
                        break;
                    }
                    return;
                    }
                default:
                    return;
            }
        }

        public virtual object GetTargetValue()
        {
            Debug.Assert(IsValid);
            return _getter();
        }

        public virtual void SetTargetValue(object value)
        {
            Debug.Assert(IsValid);
            if (!CanSetValue)
                throw new Exception("TargetMember is get only!");
            _setter(value);
        }

#if UNITY_EDITOR
        public virtual void OnValidate()
        {
            OnValueChanged();
        }

        protected virtual void OnValueChanged()
        {
            if (IncludePrivate)
                MemberBindingFlags |= BindingFlags.NonPublic;
            else
                MemberBindingFlags &= ~BindingFlags.NonPublic;

            InitializeAndCheckValid();
        }

        protected virtual IEnumerable GetTargetComponentsDropdown()
        {
            var total = new ValueDropdownList<Component>() { { "None", null } };
            if (TargetObject == null)
                return total;
            total.AddRange(TargetObject.GetComponents<Component>()
                .Select(x => new ValueDropdownItem<Component>(x.GetType().Name, x)));
            return total;
        }

        protected virtual IEnumerable GetComponentMemberNamesDropdown()
        {
            var total = new ValueDropdownList<string>() { { "None", string.Empty } };
            if (TargetComponent == null)
                return total;
             
            var masks = MemberTypeMasks;
            // 如果包含Default
            if (((int)MemberTypeMasks & 1) == 0)
                masks |= ValuePickerMemberTypeMasks.Variable;
            
            if ((masks & ValuePickerMemberTypeMasks.Field) != 0)
            {
                total.AddRange(TargetComponent.GetType().GetFields(MemberBindingFlags).Where(MemberFilter).Select(MemberSelector));
            }

            if ((masks & ValuePickerMemberTypeMasks.AllProperty) != 0)
            {
                var properties = TargetComponent.GetType().GetProperties(MemberBindingFlags);

                if ((masks & ValuePickerMemberTypeMasks.ReadOnlyProperty) != 0)
                {
                    total.AddRange(properties.Where(x => x.CanRead && !x.CanWrite).Where(MemberFilter).Select(MemberSelector));
                }
                if ((masks & ValuePickerMemberTypeMasks.WritOnlyProperty) != 0)
                {
                    total.AddRange(properties.Where(x => !x.CanRead && x.CanWrite).Where(MemberFilter).Select(MemberSelector));
                }
                if ((masks & ValuePickerMemberTypeMasks.ReadAndWriteProperty) != 0)
                {
                    total.AddRange(properties.Where(x => x.CanRead && x.CanWrite).Where(MemberFilter).Select(MemberSelector));
                }
            }
            if ((MemberTypeMasks & ValuePickerMemberTypeMasks.Method) != 0)
            {
                total.AddRange(TargetComponent.GetType().GetMethods(MemberBindingFlags)
                    .Where(x => x.GetParameters().IsNullOrEmpty())
                    .Select(MemberSelector)
                );
            }
            return total;
        }


        protected virtual ValueDropdownItem<string> MemberSelector(MemberInfo member)
        {
            string typeName;
            string stuff = string.Empty;
            Type memberType;
            bool isPrivate = false;
            switch (member)
            {
                case FieldInfo field:
                    typeName = "Field";
                    memberType = field.FieldType;
                    isPrivate = field.IsPrivate;
                    break;
                case MethodInfo method:
                    typeName = "Method";
                    memberType = method.ReturnType;
                    isPrivate = method.IsPrivate;
                    break;
                case PropertyInfo property:
                    typeName = "Property";
                    memberType = property.PropertyType;
                    stuff = property.CanRead switch
                    {
                        true when !property.CanWrite => " [ReadOnly]",
                        false when property.CanWrite => " [WriteOnly]",
                        _ => " [ReadAnyWrite]"
                    };
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (isPrivate)
                stuff += " [Private]";

            var path = $"{typeName} / {memberType.Name} / {member.Name}{stuff}";

            return new(path, $"{typeName}/{member.Name}");
        }

        protected virtual bool MemberFilter(MemberInfo member)
        {
            var filters = ValueTypeFilters;
            if (((int)filters & 1) == 0)
                filters = ValuePickerValueTypeFilters.All;

            var valueType = ReflectHelper.GetMemberValueType(member);
            if ((filters & ValuePickerValueTypeFilters.Primitive) != 0)
            {
                if (valueType.IsPrimitive)
                    return true;
            }
            if ((filters & ValuePickerValueTypeFilters.UnityObject) != 0)
            {
                if (ReflectHelper.IsUnityObject(valueType))
                    return true;
            }

            return (filters & ValuePickerValueTypeFilters.CSharpObject) != 0;
        }
#endif
    }
}
