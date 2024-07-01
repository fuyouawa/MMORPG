using MMORPG.Common.Inventory;
using MMORPG.Common.Proto.Inventory;
using MMORPG.Command;
using UnityEngine;
using UnityEngine.UI;
using QFramework;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.System;
using MMORPG.Common.Proto.Map;
using NotImplementedException = System.NotImplementedException;
using MoonSharp.VsCodeDebugger.SDK;

namespace MMORPG.UI
{
	public class UIPlayerKnapsackPanel : UIInventoryBase, IController
    {
        public RectTransform SlotsGroup;

        protected override string SlotAssetName => "UIPrefab/UIGameHub/UIKnapsackSlot";
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

            var Network = this.GetSystem<INetworkSystem>();
            Network.Receive<InventoryQueryResponse>(OnInventoryQueryResponse).UnRegisterWhenGameObjectDestroyed(gameObject);

            this.RegisterEvent<LoadKnapsackEvent>(e =>
            {
                ReloadKnapsack(e.KnapsackInfo);
            });
        }

        private void OnInventoryQueryResponse(InventoryQueryResponse response)
        {
            ReloadKnapsack(response.KnapsackInfo);
        }

        protected override void OnInstantiatedSlot(UISlotBase slot)
        {
            //var knapsackSlot = slot as UIKnapsackSlot;
            //knapsackSlot.Item = ;
        }

        private void RequestLoadKnapsack()
        {
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            this.SendCommand(new QueryInventoryCommand(playerManager.MineEntity.EntityId));
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
