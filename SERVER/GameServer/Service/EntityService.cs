using Common.Network;
using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class EntityService : ServiceBase<EntityService>
    {
        private int _serialNum = 1;
        private Dictionary<int, Entity> _entitiesSet = new();

        public Entity NewEntity(int spaceId)
        {
            lock (this)
            {
                var entity = new Entity()
                {
                    EntityId = _serialNum++,
                    Position = Vector3.Zero,
                    Direction = Vector3.Zero
                };
                _entitiesSet[entity.EntityId] = entity;
                return entity;
            }
        }

        
    }
}
