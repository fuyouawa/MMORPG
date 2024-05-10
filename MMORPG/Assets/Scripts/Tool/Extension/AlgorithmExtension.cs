using System;

namespace MMORPG.Tool
{
    public static class AlgorithmExtension
    {
        //public static T AssertNotEqual<T>(this T self, T val)
        //{
        //    Debug.Assert(!self.Equals(val));
        //    return self;
        //}

        //public static T AssertNotNull<T>(this T self)
        //{
        //    Debug.Assert(self != null);
        //    return self;
        //}

        //public static T AssertEqual<T>(this T self, T val)
        //{
        //    Debug.Assert(self.Equals(val));
        //    return self;
        //}

        public static T[] Combine<T>(this T[] array, T[] collection)
        {
            var result = new T[array.Length + collection.Length];
            Array.Copy(array, result, array.Length);
            Array.Copy(collection, 0, result, array.Length, collection.Length);
            return result;
        }

        public static object DefaultInstance(this Type type)
        {
            if (type.IsValueType)
                return Activator.CreateInstance(type);
            if (type == typeof(string))
                return string.Empty;
            return null;
        }

        // public static string ToStringAuto(this object obj)
        // {
        //     if (obj is string str)
        //         return str;
        //     var type = obj.GetType();
        //     if (type.IsValueType)
        //         return 
        // }
    }
}
