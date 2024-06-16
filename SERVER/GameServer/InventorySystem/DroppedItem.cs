using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Entity;
using GameServer.Manager;
using GameServer.Tool;
using GameServer.MapSystem;
using GameServer.EntitySystem;

namespace GameServer.InventorySystem
{
    public class DroppedItem : Entity
    {
        public int ItemId { get; private set; }
        public int Amount { get; set; }

        public DroppedItem(int entityId, int unitId, Map map, Vector3 pos, Vector3 dire, int itemId, int amount)
            : base(EntityType.DroppedItem, entityId, unitId, map, pos, dire)
        {
            Amount = amount;
            ItemId = itemId;
        }
    }
}
