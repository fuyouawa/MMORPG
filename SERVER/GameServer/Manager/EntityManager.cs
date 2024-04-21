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
        private Dictionary<int, Entity> _entityDict = new();

        public Tool.Time Time;

        public EntityManager()
        {

            CenterTimer.Instance.Register(100, UpdateAllEntity);
            Time = new();
        }

        private void UpdateAllEntity()
        {
            Time.Tick();
            foreach (Entity entity in _entityDict.Values)
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
            lock (_entityDict)
            {
                _entityDict[entity.EntityId] = entity;
            }
        }

        public void RemoveEntity(Entity entity)
        {
            lock (_entityDict)
            {
                _entityDict.Remove(entity.EntityId);
            }
        }

        public Entity? GetEntity(int entityId)
        {
            lock (_entityDict)
            {
                return _entityDict.GetValueOrDefault(entityId, null);
            }
        }

        public bool IsValidEntity(int entityId)
        {
            lock (_entityDict)
            {
                return _entityDict.ContainsKey(entityId);
            }
        }
    }
}
