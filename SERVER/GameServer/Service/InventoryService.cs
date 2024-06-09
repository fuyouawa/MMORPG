using Common.Network;
using Common.Proto.Fight;
using GameServer.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Inventory;
using GameServer.Manager;
using GameServer.Model;

namespace GameServer.Service
{
    public class InventoryService : ServiceBase<InventoryService>
    {
        public void OnHandle(NetChannel sender, InventoryQueryRequest req)
        {
            if (sender.User?.Player == null) return;
            var entity = EntityManager.Instance.GetEntity(req.EntityId);
            var player = entity as Player;
            if (player == null) return;

            player.Knapsack.AddItem(1001, 2);
            player.Knapsack.AddItem(1001, 100);

            var resp = new InventoryQueryResponse()
            {
                EntityId = player.EntityId,
            };
            if (req.QueryKnapsack)
            {
                resp.KnapsackInfo = player.Knapsack.GetInventoryInfo();
            }
            sender.Send(resp);
        }
    }
}
