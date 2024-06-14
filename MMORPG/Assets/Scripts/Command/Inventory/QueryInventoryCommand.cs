using MMORPG.Common.Proto.Inventory;
using MMORPG.Event;
using MMORPG.System;
using QFramework;

namespace MMORPG.Command
{
    public class QueryInventoryCommand : AbstractCommand
    {
        private int _entityId;

        public QueryInventoryCommand(int entityId)
        {
            _entityId = entityId;
        }

        protected override async void OnExecute()
        {
            var net = this.GetSystem<INetworkSystem>();
            var playerManager = this.GetSystem<IPlayerManagerSystem>();
            net.SendToServer(new InventoryQueryRequest()
            {
                EntityId = playerManager.MineEntity.EntityId,
                QueryKnapsack = true,
            });
            var response = await net.ReceiveAsync<InventoryQueryResponse>();
            this.SendEvent<LoadKnapsackEvent>(new(response.KnapsackInfo){});
            //response.KnapsackInfo
        }
    }
}
