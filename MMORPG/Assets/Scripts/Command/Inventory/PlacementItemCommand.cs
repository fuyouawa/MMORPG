using MMORPG.Common.Proto.Inventory;
using MMORPG.Event;
using MMORPG.System;
using QFramework;

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
        }
    }
}
