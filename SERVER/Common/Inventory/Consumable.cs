using System.Collections;
using System.Collections.Generic;

namespace MMORPG.Common.Inventory
{
    /// <summary>
    /// 消耗品
    /// </summary>
    public class Consumable : Item
    {
        public Consumable(ItemDefine define, int amount = 1, int slotId = 0) : base(define, amount, slotId)
        {
        }
    }

}