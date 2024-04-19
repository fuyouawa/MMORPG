using Common.Network;
using Common.Tool;
using GameServer.Unit;
using GameServer.Network;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using GameServer.System;

namespace GameServer.Manager
{
    /// <summary>
    /// 实体管理器
    /// 负责管理整个游戏的所有实体
    /// 线程安全
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int _serialNum = 0;
        private ConcurrentDictionary<int, Entity> _entitiesDict = new();

        public Tool.Time Time;

        public EntityManager()
        {

            CenterTimer.Instance.Register(100, Update);
        }

        private void Update()
        {
            Time.Tick();
            foreach (Entity entity in _entitiesDict.Values)
            {
                entity.Update();
            }
        }

        public int NewEntityId()
        {
            return Interlocked.Increment(ref _serialNum);
        }

        public void AddEntity(Entity entity)
        {
            _entitiesDict[entity.EntityId] = entity;
        }

        public void RemoveEntity(Entity entity)
        {
            _entitiesDict.TryRemove(entity.EntityId, out Entity outEntity);
        }

        public Entity? GetEntity(int entityId)
        {
            return _entitiesDict.GetValueOrDefault(entityId, null);
        }
    }
}
