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
using GameServer.Manager;
using MMORPG.Common.Proto.Base;

namespace GameServer.NetService
{
    public class InventoryService : ServiceBase<InventoryService>
    {
        public void OnHandle(NetChannel sender, InventoryQueryRequest req)
        {
            UpdateManager.Instance.AddTask(() =>
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
            });
        }

        public void OnHandle(NetChannel sender, PlacementItemRequest req)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                if (sender.User == null || sender.User.Player == null) return;
                var player = sender.User.Player;
                if (player.EntityId != req.EntityId) return;

                if (req.OriginSlotId == -1) return;
                if (req.TargetSlotId == -1)
                {
                    var item = player.Knapsack.GetItemBySlot(req.OriginSlotId);
                    if (item == null) return;

                    player.Map.DroppedItemManager.NewDroppedItem(item.Id, player.Position.ToVector3(), player.Direction,
                        item.Amount);
                    player.Knapsack.RemoveItemBySlot(req.OriginSlotId, int.MaxValue);
                }
                else
                {
                    player.Knapsack.Exchange(req.OriginSlotId, req.TargetSlotId);
                }
                ResponseKnapsackInfo(sender, player);
            });
        }

        private void ResponseKnapsackInfo(NetChannel sender, Player player)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                var resp = new InventoryQueryResponse()
                {
                    EntityId = player.EntityId,
                };
                resp.KnapsackInfo = player.Knapsack.GetInventoryInfo();
                sender.Send(resp);
            });
        }

        public void OnHandle(NetChannel sender, PickupItemRequest req)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                var resp = new PickupItemResponse()
                {
                    Error = NetError.Success
                };
                Player? player = null;
                do
                {
                    if (sender.User == null || sender.User.Player == null) return;
                    player = sender.User.Player;
                    // 查找距离最近的物体

                    var entity =
                        player.Map.GetEntityFollowingNearest(player, entity => entity.EntityType == EntityType.DroppedItem);
                    if (entity == null)
                    {
                        resp.Error = NetError.InvalidEntity;
                        break;
                    }

                    var droppedItem = entity as DroppedItem;
                    if (droppedItem == null)
                    {
                        resp.Error = NetError.InvalidEntity;
                        break;
                    }

                    var distance = Vector2.Distance(player.Position, droppedItem.Position);
                    if (distance > 1)
                    {
                        resp.Error = NetError.InvalidEntity;
                        break;
                    }

                    int amount = player.Knapsack.AddItem(droppedItem.ItemId, droppedItem.Amount);
                    if (amount == droppedItem.Amount)
                    {
                        resp.Error = NetError.InsufficientKnapsackCapacity;
                        break;
                    }
                    if (amount == 0)
                    {
                        player.Map.DroppedItemManager.RemoveDroppedItem(droppedItem);
                        player.Map.EntityLeave(droppedItem);
                        resp.Amount = droppedItem.Amount;
                    }
                    else
                    {
                        resp.Amount = droppedItem.Amount - amount;
                        droppedItem.Amount = amount;
                    }
                    resp.ItemId = droppedItem.ItemId;
                } while (false);
                sender.Send(resp, null);
                if (resp.Error == NetError.Success)
                {
                    ResponseKnapsackInfo(sender, player);
                }
            });
        }

        public void OnHandle(NetChannel sender, UseItemRequest req)
        {
            UpdateManager.Instance.AddTask(() =>
            {
                if (sender.User == null || sender.User.Player == null) return;
                var player = sender.User.Player;

                var item = player.Knapsack.GetItemBySlot(req.SlotId);
                if (item == null) return;
                if (!item.Define.CanUse) return;
                player.BuffManager.AddBuff(item.Define.UseBuff);
                player.Knapsack.RemoveItemBySlot(req.SlotId, 1);
                ResponseKnapsackInfo(sender, player);
            });
        }
    }
}
