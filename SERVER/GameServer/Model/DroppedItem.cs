using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;
using GameServer.Manager;
using GameServer.Tool;

namespace GameServer.Model
{
    public class DroppedItem : Entity
    {
        public int ItemId { get; private set; }
        public int Amount { get; set; }

        public DroppedItem(int entityId, Map map, int unitId, int itemId, int amount)
            : base(EntityType.DroppedItem, entityId, unitId, map)
        {
            Amount = amount;
            ItemId = itemId;
        }
    }
}
