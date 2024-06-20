using System;
using QFramework;
using System.Collections.Generic;
using MMORPG.Common.Proto.Entity;
using MMORPG.Event;
using MMORPG.Game;
using PimDeWitte.UnityMainThreadDispatcher;
using Serilog;
using UnityEngine;

namespace MMORPG.System
{
    public interface IEntityManagerSystem : ISystem
    {
        public EntityView SpawnEntity(
            EntityView prefab,
            int entityId,
            int unitId,
            EntityType type,
            Vector3 position,
            Quaternion rotation);

        public Dictionary<int, EntityView> EntityDict { get; }

        public void LeaveEntity(int entityId);
    }


    public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
    {
        public Dictionary<int, EntityView> EntityDict { get; } = new();

        private IDataManagerSystem _dataManager;

        public void LeaveEntity(int entityId)
        {
            var entity = EntityDict[entityId];
            var suc = EntityDict.Remove(entity.EntityId);
            Debug.Assert(suc);
            this.SendEvent(new EntityLeaveEvent(entity));
            Log.Information($"实体退出地图: id:{entityId}, type:{entity.EntityType}");
            // 主要为了延迟下一帧调用, 以便可以先处理EntityLeaveEvent再Destroy
            UnityMainThreadDispatcher.Instance().Enqueue(() => GameObject.Destroy(entity.gameObject));
        }

        public EntityView SpawnEntity(
            EntityView prefab,
            int entityId,
            int unitId,
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
            var unitDefine = _dataManager.GetUnitDefine(unitId);
            entity.Initialize(entityId, unitDefine);

            entity.gameObject.name = $"{unitDefine.Name}_{entityId}_{unitDefine.Kind}";

            EntityDict[entity.EntityId] = entity;

            Log.Information($"实体生成成功: id:{entityId}, type:{type}, position:{position}, rotation:{rotation}");
            this.SendEvent(new EntityEnterEvent(entity));
            return entity;
        }

        protected override void OnInit()
        {
            this.RegisterEvent<ExitedMapEvent>(OnExitedMap);

            _dataManager = this.GetSystem<IDataManagerSystem>();
        }

        private void OnExitedMap(ExitedMapEvent e)
        {
            // foreach (var entity in EntityDict)
            // {
            //     if (entity.Value.gameObject != null)
            //     {
            //         GameObject.Destroy(entity.Value.gameObject);
            //     }
            // }
            EntityDict.Clear();
        }

        protected override void OnDeinit()
        {
            EntityDict.Clear();
        }
    }
}
