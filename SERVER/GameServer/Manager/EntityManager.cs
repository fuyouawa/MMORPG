using Common.Network;
using Common.Tool;
using GameServer.Model;
using GameServer.Network;
using System;
using System.Collections.Concurrent;
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
        private ConcurrentDictionary<int, Entity> _entitiesSet = new();

        public int NewEntityId()
        {
            return Interlocked.Increment(ref _serialNum);
        }

        public void AddEntity(Entity entity)
        {
            _entitiesSet[entity.EntityId] = entity;
        }

        public void RemoveEntity(int entityId)
        {
            _entitiesSet.TryRemove(entityId, out Entity entity);
        }

        public Entity? GetEntity(int entityId)
        {
            return _entitiesSet.GetValueOrDefault(entityId, null);
        }
    }
}
