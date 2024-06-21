using UnityEngine;

namespace MMORPG.Tool
{
    public static class MathHelper
    {
        /// <summary>
        /// 将一个在[A, B]区间的x, 映射到[C, D]区间
        /// </summary>
        /// <returns></returns>
        public static float Remap(float x, float A, float B, float C, float D)
        {
            return C + (x - A) / (B - A) * (D - C);
        }

        public static bool Approximately(Quaternion a, Quaternion b, float similarityThreshold = 0.99f)
        {
            var dot = Quaternion.Dot(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return dot >= threshold;
        }

        public static bool Approximately(Vector3 a, Vector3 b, float similarityThreshold = 0.99f)
        {
            var distance = Vector3.Distance(a, b);
            var threshold = Mathf.Clamp(similarityThreshold, 0f, 1f);
            return distance <= 1 - threshold;
        }
    }
}
