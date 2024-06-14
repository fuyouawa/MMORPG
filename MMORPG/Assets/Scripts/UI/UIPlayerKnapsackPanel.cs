using MMORPG.Common.Inventory;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Command;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.System;

namespace MMORPG.UI
{
	public class UIPlayerKnapsackPanel : UIInventoryBase, IController
    {
        public RectTransform SlotsGroup;

        protected override string SlotAssetName => "UIKnapsackSlot";
        public override RectTransform GroupSlots => SlotsGroup;

        private void Awake()
        {

        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        private void Start()
        {
            RequestLoadKnapsack();
        }

        protected override void OnInstantiatedSlot(UISlotBase slot)
        {
            //var knapsackSlot = slot as UIKnapsackSlot;
            //knapsackSlot.Item = ;
        }

        private void RequestLoadKnapsack()
        {
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            this.SendCommand<QueryInventoryCommand>(new QueryInventoryCommand(playerManager.MineEntity.EntityId));
            this.RegisterEvent<LoadKnapsackEvent>(e =>
            {
                ReloadKnapsack(e.KnapsackInfo);
            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void ReloadKnapsack(InventoryInfo knapsackInfo)
        {
            SetSlotCount(knapsackInfo.Capacity);
            foreach (var itemInfo in knapsackInfo.Items)
            {
                var dataManagerSystem = this.GetSystem<IDataManagerSystem>();
                var item = new Item(dataManagerSystem.GetItemDefine(itemInfo.ItemId), itemInfo.Amount, itemInfo.SlotId);
                var slotTransform = GroupSlots.GetChild(itemInfo.SlotId);
                var slot = slotTransform.GetComponent<UIKnapsackSlot>();
                slot.Assign(item);
            }
        }
    }
}
