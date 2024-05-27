// using Sirenix.OdinInspector;
// using System;
// using System.Reflection;
//
// namespace MMORPG.Tool
// {
//     [Serializable]
//     public class InlineValueGetter : InlineMemberPicker
//     {
//         public object GetRawValue(object obj)
//         {
//             switch (TargetMember)
//             {
//                 case FieldInfo field:
//                     return field.GetValue(obj);
//                 case PropertyInfo property:
//                     return property.GetValue(obj);
//                 case MethodInfo method:
//                     return method.Invoke(obj, null);
//                 default:
//                     throw new NotSupportedException();
//             }
//         }
//
// #if UNITY_EDITOR
//         protected override bool MemberFilter(MemberInfo member)
//         {
//             if (!ReflectHelper.IsGeneralMember(member))
//                 return false;
//             if (ReflectHelper.GetGeneralMemberValueType(member) == typeof(void))
//                 return false;
//             if (member is MethodInfo method)
//                 return method.GetParameters().Length == 0 && !method.IsSpecialName;
//             return true;
//         }
// #endif
//     }
//
//     [Serializable]
//     public class InlineValueGetter<TReturn> : InlineValueGetter
//     {
//         public new TReturn GetRawValue(object obj)
//         {
//             return (TReturn)base.GetRawValue(obj);
//         }
//
// #if UNITY_EDITOR
//         protected override bool MemberFilter(MemberInfo member)
//         {
//             if (base.MemberFilter(member))
//             {
//                 return ReflectHelper.GetGeneralMemberValueType(member) == typeof(TReturn);
//             }
//             return false;
//         }
//
//         protected override ValueDropdownItem<string> MemberDropdownSelector(MemberInfo member)
//         {
//             return new($"{member.MemberType}/{member.Name}",
//                 $"{member.MemberType}/{member.Name}");
//         }
// #endif
//     }
// }
