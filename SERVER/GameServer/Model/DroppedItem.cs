using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;

namespace GameServer.Model
{
    public class DroppedItem : Entity
    {
        public int Amount;


        public DroppedItem(int entityId, int unitId,
            Map map, int amount)
            : base(EntityType.DroppedItem, entityId, unitId, map)
        {
            Amount = amount;
        }
    }
}
