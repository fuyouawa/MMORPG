using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;

namespace MMORPG.Tool
{
    public static class ReflectHelper
    {
        public static Type GetGeneralMemberValueType(MemberInfo member)
        {
            return member switch
            {
                MethodInfo method => method.ReturnType,
                FieldInfo field => field.FieldType,
                PropertyInfo property => property.PropertyType,
                _ => throw new NotImplementedException()
            };
        }

        public static bool IsIntegerType(Type type)
        {
            if (!type.IsPrimitive)
                return false;

            var typeCode = Type.GetTypeCode(type);
            return typeCode is TypeCode.SByte or TypeCode.Byte or TypeCode.Int16 or TypeCode.UInt16 or TypeCode.Int32
                or TypeCode.UInt32 or TypeCode.Int64 or TypeCode.UInt64;
        }

        public static bool IsFloatingPointType(Type type)
        {
            if (!type.IsPrimitive)
                return false;

            var typeCode = Type.GetTypeCode(type);
            return typeCode is TypeCode.Double or TypeCode.Single or TypeCode.Decimal;
        }

        public static bool IsBooleanType(Type type)
        {
            if (!type.IsPrimitive)
                return false;

            var typeCode = Type.GetTypeCode(type);
            return typeCode is TypeCode.Boolean;
        }

        public static bool IsStringType(Type type)
        {
            var typeCode = Type.GetTypeCode(type);
            return typeCode is TypeCode.String;
        }

        public static bool IsUnityObject(Type type)
        {
            return typeof(UnityEngine.Object).IsAssignableFrom(type);
        }

        public static bool IsVisualType(Type type)
        {
            return type.IsPrimitive || IsUnityObject(type);
        }

        public static MethodInfo FindMethodByName(Type targetType, string methodName, IEnumerable<string> parameterDecls, BindingFlags bindingFlags)
        {
            var paramTypesName = parameterDecls.Select(param => param.Trim().Split(' ')[0]).ToArray();

            var method = targetType.GetMethods(bindingFlags).FirstOrDefault(x =>
            {
                if (x.Name != methodName)
                    return false;
                var parameters = x.GetParameters();
                if (parameters.Length != paramTypesName.Length)
                    return false;
                return !parameters
                    .Where((t, i) => t.ParameterType.Name != paramTypesName[i])
                    .Any();
            });
            return method;
        }

        public static bool IsGeneralMember(MemberInfo member)
        {
            return member.MemberType is MemberTypes.Field or MemberTypes.Method or MemberTypes.Property;
        }
    }
}
