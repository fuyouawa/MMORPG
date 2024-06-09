
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
