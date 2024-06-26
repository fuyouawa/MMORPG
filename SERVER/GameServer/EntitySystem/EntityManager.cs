using GameServer.Tool;
using System.Security.Principal;

namespace GameServer.EntitySystem
{
    /// <summary>
    /// 实体管理器
    /// 负责管理整个游戏的所有实体
    /// </summary>
    public class EntityManager : Singleton<EntityManager>
    {
        private int _serialNum = 0;
        private Dictionary<int, Entity> _entityDict = new();

        private Dictionary<int, Entity> _addQueue = new();
        private Dictionary<int, Entity> _removeQueue = new();

        private EntityManager() { }

        public void Start() { }

        public void Update()
        {
            foreach (var entity in _entityDict.Values)
            {
                entity.Update();
            }

            foreach (var entity in _addQueue.Values)
            {
                _entityDict[entity.EntityId] = entity;
            }
            _addQueue.Clear();
            foreach (var entity in _removeQueue.Values)
            {
                _entityDict.Remove(entity.EntityId);
            }
            _removeQueue.Clear();
        }

        public int NewEntityId()
        {
            return ++_serialNum;
        }

        public void AddEntity(Entity entity)
        {
            _addQueue[entity.EntityId] = entity;
        }

        public void RemoveEntity(Entity entity)
        {
            _removeQueue[entity.EntityId] = entity;
        }

        public Entity? GetEntity(int entityId)
        {
            _entityDict.TryGetValue(entityId, out var entity);
            if (entity != null)
            {
                if (_removeQueue.ContainsKey(entityId)) return null;
            }
            else
            {
                _addQueue.TryGetValue(entityId, out entity);
            }
            return entity;
        }

        public bool IsValidEntity(int entityId)
        {
            var vaild =  _entityDict.ContainsKey(entityId);
            if (vaild) return !_removeQueue.ContainsKey(entityId);
            else return _addQueue.ContainsKey(entityId);
            
        }
    }
}
