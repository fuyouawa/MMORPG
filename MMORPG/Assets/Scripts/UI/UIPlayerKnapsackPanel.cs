using UnityEngine;
using UnityEngine.UI;
using QFramework;

namespace MMORPG.UI
{
	public class UIPlayerKnapsackPanel : UIInventoryBase
    {
        public RectTransform SlotsGroup;

        protected override string SlotAssetName => "UIKnapsackSlot";
        public override RectTransform GroupSlots => SlotsGroup;

        private void Awake()
        {
            // 初始设置20个槽
            SetSlotCount(20);
        }
    }
}
