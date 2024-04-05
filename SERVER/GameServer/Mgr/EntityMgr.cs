using GameServer.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Mgr
{
    public class EntityMgr
    {
        private int _serialNum = 1;
        private Dictionary<int, Entity> _entitiesSet = new();

        public Entity CreateEntity(int spaceId)
        {
            lock (this)
            {
                var entity = new Entity(_serialNum++, Vector3.Zero, Vector3.Zero);
                _entitiesSet[entity.EntityId] = entity;
                return entity;
            }
        }
    }
}
