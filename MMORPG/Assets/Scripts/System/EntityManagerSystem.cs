using Common.Proto.Event;
using Common.Proto.Event.Map;
using QFramework;
using System.Collections.Generic;
using MMORPG.Event;
using MMORPG.Game;
using MMORPG.Tool;
using UnityEngine;

namespace MMORPG.System
{
    public class NetworkEntityEnterEvent
    {
        public int EntityId { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public NetworkEntityEnterEvent(int entityId, Vector3 position, Quaternion rotation)
        {
            EntityId = entityId;
            Position = position;
            Rotation = rotation;
        }
    }

    public class NetworkEntitySyncEvent
    {
        public int EntityId { get; }
        public Vector3 Position { get; }
        public Quaternion Rotation { get; }

        public NetworkEntitySyncEvent(int entityId, Vector3 position, Quaternion rotation)
        {
            EntityId = entityId;
            Position = position;
            Rotation = rotation;
        }
    }

    public interface IEntityManagerSystem : ISystem
    {
        public EntityView SpawnEntity(
            EntityView prefab,
            int entityId,
            Vector3 position,
            Quaternion rotation,
            bool isMine);

        public Dictionary<int, EntityView> GetEntityDict(bool isMine);
    }


    public class EntityManagerSystem : AbstractSystem, IEntityManagerSystem
    {
        private readonly Dictionary<int, EntityView> _mineEntityDict = new();
        private readonly Dictionary<int, EntityView> _notMineEntityDict = new();

        public Dictionary<int, EntityView> GetEntityDict(bool isMine)
        {
            return isMine ? _mineEntityDict : _notMineEntityDict;
        }

        public EntityView SpawnEntity(EntityView prefab, int entityId, Vector3 position, Quaternion rotation,
            bool isMine)
        {
            Debug.Assert(
                !(_mineEntityDict.ContainsKey(entityId) ||
                  _notMineEntityDict.ContainsKey(entityId)));

            var entity = GameObject.Instantiate(prefab, position, rotation);
            entity.transform.SetPositionAndRotation(position, rotation);

            entity.SetEntityId(entityId);
            entity.SetIsMine(isMine);

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
            this.GetSystem<INetworkSystem>().ReceiveEvent<EntityEnterResponse>(OnEntityEnterReceived);
            this.GetSystem<INetworkSystem>().ReceiveEvent<EntityTransformSyncResponse>(OnEntitySyncReceived);
            this.RegisterEvent<ApplicationQuitEvent>(OnApplicationQuit);
        }

        private void OnApplicationQuit(ApplicationQuitEvent e)
        {
            _mineEntityDict.Clear();
            _notMineEntityDict.Clear();
        }

        private void OnEntityEnterReceived(EntityEnterResponse response)
        {
            foreach (var entity in response.Datas)
            {
                var e = new NetworkEntityEnterEvent(
                    entity.EntityId,
                    entity.Transform.Position.ToVector3(),
                    Quaternion.Euler(entity.Transform.Direction.ToVector3()));

                Tool.Log.Info("Game", $"实体({entity.EntityId})加入: Position:{e.Position}, Rotation:{e.Rotation}");
                this.SendEvent(e);
            }
        }

        private void OnEntitySyncReceived(EntityTransformSyncResponse response)
        {
            this.SendEvent(new NetworkEntitySyncEvent(
                response.EntityId,
                response.Transform.Position.ToVector3(),
                Quaternion.Euler(response.Transform.Direction.ToVector3())));
        }
    }
}
