
using Common.Proto.Base;
using Common.Proto.Inventory;
using Common.Proto.Map;
using Common.Proto.User;
using MMORPG.Event;
using MMORPG.Model;
using MMORPG.System;
using QFramework;
using UnityEngine.SceneManagement;

namespace MMORPG.Command
{
    public class PlacementItemCommand : AbstractCommand
    {
        private int _originSlot;
        private int _targetSlot;

        public PlacementItemCommand(int originSlot, int targetSlot)
        {
            _originSlot = originSlot;
            _targetSlot = targetSlot;
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            net.SendToServer(new PlacementItemRequest()
            {
                EntityId = playerManager.MineEntity.EntityId,
                OriginSlotId = _originSlot,
                TargetSlotId = _targetSlot,
            });
            var response = await net.ReceiveAsync<InventoryQueryResponse>();
            this.SendEvent<LoadKnapsackEvent>(new(response.KnapsackInfo){});
        }
    }
}
