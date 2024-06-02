using System;
using QFramework;
using System.Collections.Generic;
using MMORPG.Event;
using MMORPG.Game;
using PimDeWitte.UnityMainThreadDispatcher;
using UnityEngine;

namespace MMORPG.System
{
    public interface IEntityManagerSystem : ISystem
    {
        public EntityView SpawnEntity(
            EntityView prefab,
            int entityId,
            EntityType type,
            Vector3 position,
            Quaternion rotation);

        public Dictionary<int, EntityView> EntityDict { get; }

        public void LeaveEntity(int entityId);
        public EntityView GetEntityById(int entityId);
    }


    public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
    {
        public Dictionary<int, EntityView> EntityDict { get; } = new();

        public void LeaveEntity(int entityId)
        {
            var entity = GetEntityById(entityId);
            var suc = EntityDict.Remove(entity.EntityId);
            Debug.Assert(suc);
            this.SendEvent(new EntityLeaveEvent(entity));
            Tool.Log.Info("Game", $"实体退出地图: id:{entityId}, type:{entity.EntityType}");
            // 主要为了延迟下一帧调用, 以便可以先处理EntityLeaveEvent再Destroy
            UnityMainThreadDispatcher.Instance().Enqueue(() => GameObject.Destroy(entity.gameObject));
        }

        public EntityView GetEntityById(int entityId)
        {
            if (!EntityDict.TryGetValue(entityId, out var entity))
                throw new Exception($"未注册过的entityId:{entityId}!");
            return entity;
        }

        public EntityView SpawnEntity(
            EntityView prefab,
            int entityId,
            EntityType type,
            Vector3 position,
            Quaternion rotation)
        {
            Debug.Assert(!EntityDict.ContainsKey(entityId));

            if (prefab.EntityType != type)
            {
                throw new Exception("EntityType与当前预制体的Type不相同!");
            }

            var entity = GameObject.Instantiate(prefab, position, rotation);
            entity.transform.SetPositionAndRotation(position, rotation);
            entity.Initialize(entityId);

            EntityDict[entity.EntityId] = entity;

            Tool.Log.Info("Game", $"实体生成成功: id:{entityId}, type:{type}, position:{position}, rotation:{rotation}");
            this.SendEvent(new EntityEnterEvent(entity));
            return entity;
        }

        protected override void OnInit()
        {
            this.RegisterEvent<ExitedMapEvent>(OnExitedMap);
        }

        private void OnExitedMap(ExitedMapEvent e)
        {
            foreach (var entity in EntityDict)
            {
                GameObject.Destroy(entity.Value.gameObject);
            }
            EntityDict.Clear();
        }
    }
}
