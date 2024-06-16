using System;
using System.Diagnostics;
using System.Linq;
using System.Numerics;

namespace MMORPG.Common.Tool
{
    public static class DataHelper
    {
        public static Vector3 ParseVector3(string str)
        {
            var floats = ParseFloats(str);
            if (floats.Length == 0)
            {
                return Vector3.Zero;
            }
            Debug.Assert(floats.Length == 3);
            Vector3 res;
            res.X = floats[0];
            res.Y = floats[1];
            res.Z = floats[2];
            return res;
        }

        public static float[] ParseFloats(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
                return Array.Empty<float>();
            return str[1..^1].Split(',').Select(float.Parse).ToArray();
        }
    }
}