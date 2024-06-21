using MMORPG.Common.Proto.Inventory;
using MMORPG.Event;
using MMORPG.System;
using QFramework;

namespace MMORPG.Command
{
    public class PickupItemCommand : AbstractCommand
    {
        public PickupItemCommand()
        {
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            net.SendToServer(new PickupItemRequest()
            {
            });
            var response = await net.ReceiveAsync<PickupItemResponse>();
            this.SendEvent(new PickupItemEvent(response){});
        }
    }
}
