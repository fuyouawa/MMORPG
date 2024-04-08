using Common.Network;
using Common.Tool;
using GameServer.Model;
using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    public class EntityManager : Singleton<EntityManager>
    {
        private int _serialNum = 0;
        private Dictionary<int, Entity> _entitiesSet = new();

        public int NewEntityId()
        {
            return Interlocked.Increment(ref _serialNum);
        }

        public void AddEntity(Entity entity)
        {
            lock (_entitiesSet)
            {
                _entitiesSet[entity.EntityId] = entity;
            }
        }
    }
}
