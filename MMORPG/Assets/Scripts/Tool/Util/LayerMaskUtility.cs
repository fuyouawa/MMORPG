using UnityEngine;

namespace MMORPG.Tool
{
    /// <summary>
    /// LayerMask 扩展方法工具类
    /// </summary>
    public static class LayerMaskUtility
    {
        /// <summary>
        /// 检查指定层级是否在图层遮罩中
        /// </summary>
        /// <param name="mask">图层遮罩</param>
        /// <param name="layer">层级索引</param>
        /// <returns>如果层级在遮罩中则返回 true</returns>
        public static bool IsInLayerMask(this LayerMask mask, int layer)
        {
            return (mask.value & (1 << layer)) > 0;
        }

        /// <summary>
        /// 检查游戏对象的层级是否在图层遮罩中
        /// </summary>
        /// <param name="mask">图层遮罩</param>
        /// <param name="obj">游戏对象</param>
        /// <returns>如果游戏对象的层级在遮罩中则返回 true</returns>
        public static bool IsInLayerMask(this LayerMask mask, GameObject obj)
        {
            return (mask.value & (1 << obj.layer)) > 0;
        }
    }
}
