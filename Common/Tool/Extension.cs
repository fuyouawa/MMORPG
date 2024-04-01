using System.Diagnostics;

namespace Common.Tool
{
    public static class Extension
    {
        public static T AssertNotNull<T>(this T?  value)
        {
            Debug.Assert(value != null);
            return value;
        }
    }
}
