using Common.Inventory;
using QFramework;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIKnapsackSlot : UISlotBase
    {
        public UIItem UIItem;

        /// <summary>
        /// 将Item赋值给此槽
        /// </summary>
        /// <param name="item"></param>
        public void Assign(Item item)
        {

            Debug.Assert(item.SlotId == SlotId);
            UIItem.Assign(item);

        }

        /// <summary>
        /// 将此槽置空
        /// </summary>
        public void SetEmpty()
        {
            UIItem.Clear();
        }



        
        //TODO 右键菜单
    }
}
