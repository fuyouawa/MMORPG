using System;
using System.Reflection;

namespace MMORPG.Tool
{
    public static class ReflectHelper
    {
        public static Type GetMemberValueType(MemberInfo member)
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
    }
}
