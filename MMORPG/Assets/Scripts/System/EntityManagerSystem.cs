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
            bool isMine,
            Vector3 position,
            Quaternion rotation);

        public void LeaveEntity(int entityId);

        public Dictionary<int, EntityView> GetEntityDict(bool isMine);
        public EntityView GetEntityById(int entityId);
    }


    public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
    {
        private readonly Dictionary<int, EntityView> _mineEntityDict = new();
        private readonly Dictionary<int, EntityView> _notMineEntityDict = new();

        public void LeaveEntity(int entityId)
        {
            var entity = GetEntityById(entityId);
            var suc = (entity.IsMine ? _mineEntityDict : _notMineEntityDict).Remove(entity.EntityId);
            Debug.Assert(suc);
            this.SendEvent(new EntityLeaveEvent(entity));
            // 主要为了延迟下一帧调用, 以便可以先处理EntityLeaveEvent再Destroy
            UnityMainThreadDispatcher.Instance().Enqueue(() => GameObject.Destroy(entity.gameObject));
        }

        public Dictionary<int, EntityView> GetEntityDict(bool isMine)
        {
            return isMine ? _mineEntityDict : _notMineEntityDict;
        }

        public EntityView GetEntityById(int entityId)
        {
            if (_mineEntityDict.TryGetValue(entityId, out var entity))
                return entity;
            if (!_notMineEntityDict.TryGetValue(entityId, out entity))
                throw new Exception($"未注册过的entityId:{entityId}!");
            return entity;
        }

        public EntityView SpawnEntity(
            EntityView prefab,
            int entityId,
            EntityType type,
            bool isMine,
            Vector3 position,
            Quaternion rotation)
        {
            Debug.Assert(
                !(_mineEntityDict.ContainsKey(entityId) ||
                  _notMineEntityDict.ContainsKey(entityId)));

            var entity = GameObject.Instantiate(prefab, position, rotation);
            entity.transform.SetPositionAndRotation(position, rotation);
            entity.Initialize(entityId, type, isMine);

            if (entity.IsMine)
            {
                _mineEntityDict[entity.EntityId] = entity;
            }
            else
            {
                _notMineEntityDict[entity.EntityId] = entity;
            }

            Tool.Log.Info("Game", $"实体生成成功: id:{entityId}, position:{position}, rotation:{rotation}, isMine:{isMine}");
            this.SendEvent(new EntityEnterEvent(entity));
            return entity;
        }

        protected override void OnInit()
        {
            this.RegisterEvent<ExitedMapEvent>(OnExitedMap);
        }

        private void OnExitedMap(ExitedMapEvent e)
        {
            _mineEntityDict.Clear();
            _notMineEntityDict.Clear();
        }
    }
}
