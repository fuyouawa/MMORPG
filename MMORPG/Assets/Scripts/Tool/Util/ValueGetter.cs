using System;
using System.Reflection;
using Sirenix.OdinInspector;

namespace MMORPG.Tool
{
    [Serializable]
    public class ValueGetter : MemberPicker
    {
        public object GetRawValue()
        {
            switch (TargetMember)
            {
                case FieldInfo field:
                    return field.GetValue(TargetComponent);
                case PropertyInfo property:
                    return property.GetValue(TargetComponent);
                case MethodInfo method:
                    return method.Invoke(TargetComponent, null);
                default:
                    throw new NotSupportedException();
            }
        }

#if UNITY_EDITOR
        protected override bool MemberFilter(MemberInfo member)
        {
            if (!ReflectHelper.IsGeneralMember(member))
                return false;
            if (ReflectHelper.GetGeneralMemberValueType(member) == typeof(void))
                return false;
            if (member is MethodInfo method)
                return method.GetParameters().Length == 0 && !method.IsSpecialName;
            return true;
        }
#endif
    }

    [Serializable]
    public class ValueGetter<TReturn> : ValueGetter
    {
        public new TReturn GetRawValue()
        {
            return (TReturn)base.GetRawValue();
        }

#if UNITY_EDITOR
        protected override bool MemberFilter(MemberInfo member)
        {
            if (base.MemberFilter(member))
            {
                return ReflectHelper.GetGeneralMemberValueType(member) == typeof(TReturn);
            }
            return false;
        }

        protected override ValueDropdownItem<string> MemberDropdownSelector(MemberInfo member)
        {
            return new($"{member.MemberType}/{member.Name}",
                $"{member.MemberType}/{member.Name}");
        }
#endif
    }
}
