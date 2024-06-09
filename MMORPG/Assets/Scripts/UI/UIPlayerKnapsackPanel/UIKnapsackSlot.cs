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
                if (TextAmount != null)
                {
                    TextAmount.SetText(value == 0 ? string.Empty : value.ToString());
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
            ImageIcon.enabled = false;
            ImageIcon.sprite = null;
            Amount = 0;
            Item = null;
        }
    }
}
