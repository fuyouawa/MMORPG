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
    }
}
