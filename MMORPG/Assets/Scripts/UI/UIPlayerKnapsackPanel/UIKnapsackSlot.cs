using Common.Inventory;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MMORPG.UI
{
    public class UIKnapsackSlot : UISlotBase
    {
        public TextMeshProUGUI TextAmount;
        public Image ImageIcon;

        /// <summary>
        /// Item信息
        /// </summary>
        public Item Item { get; protected set; }

        private int _amount;

        /// <summary>
        /// 物品数量
        /// </summary>
        public int Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                // 显示到UI上
                if (TextAmount != null)
                {
                    TextAmount.SetText(value == 0 ? string.Empty : value.ToString());
                }
                // 同步更新Item中的Amount
                if (Item != null)
                {
                    Item.Amount = value;
                }
            }
        }

        /// <summary>
        /// 将Item赋值给此槽
        /// </summary>
        /// <param name="item"></param>
        public void Assign(Item item)
        {
            Debug.Assert(item.SlotId == SlotId);
            Amount = item.Amount;
            // 根据Item中的Icon显示物品图标
            ImageIcon.enabled = true;
            ImageIcon.sprite = Resources.Load<Sprite>(item.Icon);
            //TODO 其他可能的处理
            Item = item;
        }

        /// <summary>
        /// 将此槽置空
        /// </summary>
        public void SetEmpty()
        {
            // 隐藏物品图标
            ImageIcon.enabled = false;
            ImageIcon.sprite = null;
            Amount = 0;
            //TODO 其他可能的处理
            Item = null;
        }

        //TODO 右键菜单
    }
}
