using Common.Network;
using Common.Proto.Fight;
using GameServer.Network;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;
using Common.Proto.Inventory;
using GameServer.Model;
using GameServer.Tool;

namespace GameServer.Service
{
    public class DroppedItemService : ServiceBase<DroppedItemService>
    {
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
            
            player.Knapsack.AddItem(droppedItem.ItemId);

            player.Map.DroppedItemManager.RemoveDroppedItem(droppedItem);
            player.Map.EntityLeave(droppedItem);
        }

        public void OnHandle(NetChannel sender, DiscardItemRequest req)
        {

        }
    }
}
