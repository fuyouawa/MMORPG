using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Sirenix.OdinInspector;

namespace MMORPG.Tool
{
    [Flags]
    public enum MethodPickerParamTypeFilters
    {
        Default = 0,
        Primitive = 1,
        UnityObject = 1 << 1,
        CSharpObject = 1 << 2,
        Visual = Primitive | UnityObject,
        All = Primitive | UnityObject | CSharpObject
    }
    [Serializable]
    public class MethodPicker : MemberPicker
    {
        [PropertyOrder(11)]
        public bool IncludeSpecial = false;
        [PropertyOrder(12)]
        public bool LimitParam = false;
        [PropertyOrder(13)]
        [ShowIf("LimitParam")]
        public int ParamCount = 3;

        public MethodInfo TargetMethod => TargetMember as MethodInfo;

        public bool ShowTreeInValueDropdownInspector { get; set; } = false;
        public MethodPickerParamTypeFilters ParamTypeFilters { get; set; }
        public override string TargetMemberLabel { get; } = "Target Method";


        public MethodPicker(bool includeSpecial = false,
            MethodPickerParamTypeFilters paramTypeFilters = MethodPickerParamTypeFilters.Default,
            BindingFlags memberBindingFlags = BindingFlags.Instance | BindingFlags.Public)
            : base(memberBindingFlags)
        {
            IncludeSpecial = includeSpecial;
            ParamTypeFilters = paramTypeFilters;
        }

        public virtual bool TryGetMethod(out MethodInfo method)
        {
            method = null;
            if (TryGetMember(out var member))
                method = member as MethodInfo;
            return method != null;
        }

        public override bool TryGetMember(out MemberInfo member)
        {
            member = null;
            if (!TargetObject || !TargetComponent || string.IsNullOrEmpty(TargetMemberName))
                return false;

            var matches = Regex.Matches(TargetMemberName, @"([a-zA-Z_]\w*)\((.*)\)");
            var methodName = matches[0].Groups[1].Value;
            var paramsStr = matches[0].Groups[2].Value;

            member = ReflectHelper.FindMethodByName(
                TargetComponent.GetType(),
                methodName,
                paramsStr.Split(',').Where(x => !string.IsNullOrEmpty(x)),
                MemberBindingFlags);

            return member != null;
        }

#if UNITY_EDITOR

        protected override bool MemberFilter(MemberInfo member)
        {
            if (member.MemberType != MemberTypes.Method)
                return false;
            var method = (MethodInfo)member;
            if (!IncludeSpecial && method.IsSpecialName)
                return false;
            var parameters = method.GetParameters();
            if (LimitParam && parameters.Length > ParamCount)
                return false;
            if (parameters.FirstOrDefault(x => !ParamTypeFilter(x.ParameterType)) != null)
                return false;
            return true;
        }

        protected override ValueDropdownItem<string> MemberDropdownSelector(MemberInfo member)
        {
            var method = (MethodInfo)member;
            var paramsName = string.Join(", ", method.GetParameters().Select(x => $"{x.ParameterType.Name} {x.Name}"));
            var methodName = $"{method.Name}({paramsName})".Trim();
            if (ShowTreeInValueDropdownInspector)
            {
                var methodPath = $"{method.ReturnType} / {methodName}";
                return new(methodPath, methodName);
            }
            else
            {
                return new(methodName, methodName);
            }
        }

        protected virtual bool ParamTypeFilter(Type type)
        {
            var filters = ParamTypeFilters;
            if (((int)filters & 1) == 0)
                filters = MethodPickerParamTypeFilters.All;

            if (filters == MethodPickerParamTypeFilters.All)
                return true;

            if ((filters & MethodPickerParamTypeFilters.Primitive) != 0)
            {
                if (type.IsPrimitive)
                    return true;
            }
            if ((filters & MethodPickerParamTypeFilters.UnityObject) != 0)
            {
                if (ReflectHelper.IsUnityObject(type))
                    return true;
            }

            return (filters & MethodPickerParamTypeFilters.CSharpObject) != 0;
        }
#endif
    }
}
