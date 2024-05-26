// using System;
// using System.Collections;
// using System.Linq;
// using System.Reflection;
// using QFramework;
// using Sirenix.OdinInspector;
// using UnityEngine;
//
// namespace MMORPG.Tool
// {
//     [Flags]
//     public enum PropertyPickerTypeMasks
//     {
//         Default = 0,
//         Field = 1,
//         ReadOnlyProperty = 1 << 1,
//         WritOnlyProperty = 1 << 2,
//         ReadAndWriteProperty = 1 << 3,
//         AllProperty = ReadOnlyProperty | WritOnlyProperty | ReadAndWriteProperty,
//         WriteableVariable = Field | WritOnlyProperty | ReadAndWriteProperty,
//         All = Field | AllProperty,
//     }
//
//     [Flags]
//     public enum PropertyPickerValueTypeFilters
//     {
//         Default = 0,
//         Primitive = 1,
//         UnityObject = 1 << 1,
//         CSharpObject = 1 << 2,
//         Graphicizable = Primitive | UnityObject,
//         All = Primitive | UnityObject | CSharpObject
//     }
//
//     //TODO PropertyPicker待测试
//     [Serializable]
//     public class PropertyPicker : MemberPicker
//     {
//         public bool CanSetValue { get; private set; }
//
//         private Func<object> _getter;
//         private Action<object> _setter;
//
//         public PropertyPickerValueTypeFilters ValueTypeFilters { get; set; }
//         public PropertyPickerTypeMasks TypeMasks { get; set; }
//
//         public override string TargetMemberLabel { get; } = "Target Property";
//
//         public PropertyPicker(
//             bool includePrivate = false,
//             PropertyPickerTypeMasks typeMasks = PropertyPickerTypeMasks.Default,
//             PropertyPickerValueTypeFilters valueTypeFilters = PropertyPickerValueTypeFilters.Default,
//             BindingFlags propertyBindingFlags = BindingFlags.Instance | BindingFlags.Public)
//             : base(includePrivate, propertyBindingFlags)
//         {
//             TypeMasks = typeMasks;
//             ValueTypeFilters = valueTypeFilters;
//         }
//
//         protected override void OnInitialized()
//         {
//             switch (TargetMember)
//             {
//                 case FieldInfo field:
//                 {
//                     CanSetValue = field.IsPublic;
//                     _getter = () => field.GetValue(TargetComponent);
//                     _setter = obj => field.SetValue(TargetComponent, obj);
//                     IsValid = true;
//                     break;
//                     }
//                 case PropertyInfo property:
//                 {
//                     CanSetValue = property.CanWrite;
//                     _getter = () => property.GetValue(TargetComponent);
//                     _setter = obj => property.SetValue(TargetComponent, obj);
//                     IsValid = true;
//                     break;
//                 }
//                 default:
//                     return;
//             }
//         }
//
//         public virtual object GetTargetValue()
//         {
//             Debug.Assert(IsValid);
//             return _getter();
//         }
//
//         public virtual void SetTargetValue(object value)
//         {
//             Debug.Assert(IsValid);
//             if (!CanSetValue)
//                 throw new Exception("TargetVariable is get only!");
//             _setter(value);
//         }
//
// #if UNITY_EDITOR
//         protected override bool MemberFilter(MemberInfo member)
//         {
//             var masks = TypeMasks;
//             // 如果包含Default
//             if (((int)TypeMasks & 1) == 0)
//                 masks |= PropertyPickerTypeMasks.All;
//             switch (member)
//             {
//                 case FieldInfo field:
//                 {
//                     if (!masks.HasFlag(PropertyPickerTypeMasks.Field))
//                         return false;
//                     return ValueTypeFilter(field.FieldType);
//                 }
//                 case PropertyInfo property:
//                 {
//                     if (!masks.HasFlag(PropertyPickerTypeMasks.AllProperty))
//                         return false;
//                     if (masks.HasFlag(PropertyPickerTypeMasks.ReadOnlyProperty))
//                     {
//                         if (property.CanRead && !property.CanWrite)
//                             return ValueTypeFilter(property.PropertyType);
//                     }
//                     if (masks.HasFlag(PropertyPickerTypeMasks.WritOnlyProperty))
//                     {
//                         if (!property.CanRead && property.CanWrite)
//                             return ValueTypeFilter(property.PropertyType);
//                     }
//                     if (masks.HasFlag(PropertyPickerTypeMasks.ReadAndWriteProperty))
//                     {
//                         if (property.CanRead && property.CanWrite)
//                             return ValueTypeFilter(property.PropertyType);
//                     }
//                     return false;
//                     }
//                 default:
//                     return false;
//             }
//         }
//
//         protected override ValueDropdownItem<string> MemberDropdownSelector(MemberInfo member)
//         {
//             string stuff = string.Empty;
//             Type memberType;
//             bool isPrivate = false;
//             switch (member)
//             {
//                 case FieldInfo field:
//                     memberType = field.FieldType;
//                     isPrivate = field.IsPrivate;
//                     break;
//                 case PropertyInfo property:
//                     memberType = property.PropertyType;
//                     stuff = property.CanRead switch
//                     {
//                         true when !property.CanWrite => " [ReadOnly]",
//                         false when property.CanWrite => " [WriteOnly]",
//                         _ => " [ReadAnyWrite]"
//                     };
//                     break;
//                 default:
//                     throw new NotSupportedException();
//             }
//
//             if (isPrivate)
//                 stuff += " [Private]";
//
//             var path = $"{member.MemberType} / {memberType.Name} / {member.Name}{stuff}";
//
//             return new(path, $"{member.MemberType}/{member.Name}");
//         }
//
//         protected virtual bool ValueTypeFilter(Type type)
//         {
//             var filters = ValueTypeFilters;
//             if (((int)filters & 1) == 0)
//                 filters = PropertyPickerValueTypeFilters.All;
//
//             if (filters == PropertyPickerValueTypeFilters.All)
//                 return true;
//
//             if ((filters & PropertyPickerValueTypeFilters.Primitive) != 0)
//             {
//                 if (type.IsPrimitive)
//                     return true;
//             }
//             if ((filters & PropertyPickerValueTypeFilters.UnityObject) != 0)
//             {
//                 if (ReflectHelper.IsUnityObject(type))
//                     return true;
//             }
//
//             return (filters & PropertyPickerValueTypeFilters.CSharpObject) != 0;
//         }
// #endif
//     }
// }
