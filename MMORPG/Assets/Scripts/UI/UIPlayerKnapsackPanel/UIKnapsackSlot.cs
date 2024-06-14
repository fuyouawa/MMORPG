using System;
using MMORPG.Common.Inventory;
using MMORPG.Game;
using UnityEngine;

namespace MMORPG.UI
{
    public class UIKnapsackSlot : UISlotBase
    {
        public UIItem UIItemPrefab;

        public UIItem UIItem { get; private set; }


        /// <summary>
        /// 将Item赋值给此槽
        /// </summary>
        /// <param name="item"></param>
        public void Assign(Item item)
        {
            Debug.Assert(item.SlotId == SlotId);

            if (UIItem != null)
            {
                SetEmpty();
            }
            UIItem = Instantiate(UIItemPrefab, transform);
            UIItem.Assign(item);

        }

        /// <summary>
        /// 将此槽置空
        /// </summary>
        public void SetEmpty()
        {
            Destroy(UIItem.transform.gameObject);
            UIItem = null;
        }

        protected override void OnShowTooltip()
        {
            if (UIItem != null && UIItem.Item != null)
            {
                PrepareTooltip();
                UIToolTip.Show(transform);
            }
        }

        private void PrepareTooltip()
        {
            UIToolTip.Cleanup();

            //TODO 颜色
            var colorQuality = UIItem.Item.Quality switch
            {
                Quality.Common => Color.white,
                Quality.Uncommon => Color.green,
                Quality.Rare => Color.yellow,
                Quality.Epic => Color.white,
                Quality.Legendary => Color.white,
                Quality.Artifact => Color.white,
                _ => throw new ArgumentOutOfRangeException()
            };

            UIToolTip.AddTitle(UIItem.Item.Name, colorQuality);
            UIToolTip.AddLine($"品质:");
            UIToolTip.AddColumn(UIItem.Item.Define.Quality, colorQuality);
            UIToolTip.AddLine($"数量:{UIItem.Amount}");
        }

        protected override void OnHideTooltip()
        {
            UIToolTip.Hide();
        }

        //TODO 右键菜单
    }
}
