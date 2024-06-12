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
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;

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

        public void OnHandle(NetChannel sender, PlacementItemRequest req)
        {
            if (sender.User == null || sender.User.Player == null) return;
            var player = sender.User.Player;
            if (player.EntityId != req.EntityId) return;

            if (req.OriginSlotId == -1) return;
            if (req.TargetSlotId == -1)
            {
                var item = player.Knapsack.GetItemBySlot(req.OriginSlotId);
                if (item == null) return;

                player.Map.DroppedItemManager.NewDroppedItem(item.Id, player.Position, player.Direction, item.Amount);
                player.Knapsack.Discard(req.OriginSlotId, int.MaxValue);
            }
            else
            {
                player.Knapsack.Exchange(req.OriginSlotId, req.TargetSlotId);
            }
            ResponseKnapsackInfo(sender, player);
        }

        private void ResponseKnapsackInfo(NetChannel sender, Player player)
        {
            var resp = new InventoryQueryResponse()
            {
                EntityId = player.EntityId,
            };
            resp.KnapsackInfo = player.Knapsack.GetInventoryInfo();
            sender.Send(resp);
        }
    }
}
