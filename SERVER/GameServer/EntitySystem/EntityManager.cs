using GameServer.Tool;

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

        private EntityManager() { }

        public void Start() { }

        public void Update()
        {
            foreach (Entity entity in _entityDict.Values)
            {
                entity.Update();
            }
        }

        public int NewEntityId()
        {
            return ++_serialNum;
        }

        public void AddEntity(Entity entity)
        {
            _entityDict[entity.EntityId] = entity;
        }

        public void RemoveEntity(Entity entity)
        {
            _entityDict.Remove(entity.EntityId);
        }

        public Entity? GetEntity(int entityId)
        {
            _entityDict.TryGetValue(entityId, out var entity);
            return entity;
        }

        public bool IsValidEntity(int entityId)
        {
            return _entityDict.ContainsKey(entityId);
        }
    }
}
