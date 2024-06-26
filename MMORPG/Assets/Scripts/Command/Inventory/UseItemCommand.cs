using MMORPG.Common.Proto.Inventory;
using MMORPG.Event;
using MMORPG.System;
using QFramework;

namespace MMORPG.Command
{
    public class UseItemCommand : AbstractCommand
    {
        private int _slotId;
        public UseItemCommand(int slotId)
        {
            _slotId = slotId;
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            net.SendToServer(new UseItemRequest()
            {
                EntityId = playerManager.MineEntity.EntityId,
                SlotId = _slotId,
            });
        }
    }
}
