using System.Collections;
using System.Collections.Generic;

namespace MMORPG.Common.Inventory
{
    /// <summary>
    /// 材料
    /// </summary>
    public class Material : Item
    {
        public Material(ItemDefine define, int amount = 1, int slotId = 0) : base(define, amount, slotId)
        {
        }
    }
}