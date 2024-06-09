using Common.Inventory;
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
            //this.RegisterEvent<PlayerJoinedMapEvent>(e =>
            //{
                var playerManager = this.GetSystem<IPlayerManagerSystem>();
                this.SendCommand<QueryInventoryCommand>(new QueryInventoryCommand(playerManager.MineEntity.EntityId));
            //}).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<LoadKnapsackEvent>(e =>
            {
                SetSlotCount(e.KnapsackInfo.Capacity);
                foreach (var itemInfo in e.KnapsackInfo.Items)
                {
                    var dataManagerSystem = this.GetSystem<IDataManagerSystem>();
                    var item = new Item(dataManagerSystem.GetItemDefine(itemInfo.ItemId), itemInfo.Amount, itemInfo.SlotId);

                    var slotTransform = GroupSlots.GetChild(itemInfo.SlotId);
                    var slot = slotTransform.GetComponent<UIKnapsackSlot>();
                    slot.Assign(item);

                    //    itemInfo
                }

            }).UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        protected override void OnInstantiatedSlot(UISlotBase slot)
        {
            //var knapsackSlot = slot as UIKnapsackSlot;
            //knapsackSlot.Item = ;
        }
    }
}
