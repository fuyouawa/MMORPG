using MMORPG.Common.Network;
using MMORPG.Common.Proto.Fight;
using GameServer.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Inventory;
using Microsoft.Extensions.DependencyInjection;
using System.Numerics;
using GameServer.EntitySystem;
using GameServer.InventorySystem;
using GameServer.Tool;
using MMORPG.Common.Proto.Entity;
using GameServer.PlayerSystem;

namespace GameServer.NetService
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

        public void OnHandle(NetChannel sender, PickupItemRequest req)
        {
            if (sender.User == null || sender.User.Player == null) return;
            var player = sender.User.Player;
            // 查找距离最近的物体

            var entity = player.Map.GetEntityFollowingNearest(player, entity => entity.EntityType == EntityType.DroppedItem);
            if (entity == null) return;

            var droppedItem = entity as DroppedItem;
            if (droppedItem == null) return;

            var distance = Vector2.Distance(player.Position.ToVector2(), droppedItem.Position.ToVector2());
            if (distance > 1) return;

            int amount = player.Knapsack.AddItem(droppedItem.ItemId, droppedItem.Amount);

            if (amount == 0)
            {
                player.Map.DroppedItemManager.RemoveDroppedItem(droppedItem);
            }
            else
            {
                droppedItem.Amount = amount;
            }
            player.Map.EntityLeave(droppedItem);
        }

        public void OnHandle(NetChannel sender, DiscardItemRequest req)
        {

        }
    }
}
