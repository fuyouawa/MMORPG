using Common.Proto.Event;
using Common.Proto.Event.Map;
using QFramework;
using MMORPG.System;
using MMORPG.Tool;
using UnityEngine;

namespace MMORPG.Game
{
    public struct EntityTransformSyncData
    {
        public EntityView Entity;
        public Vector3 Position;
        public Quaternion Rotation;
        public int StateId;
        public byte[] Data;
    }

    public class EntityManager : MonoBehaviour, IController, ICanSendEvent
    {
        private IEntityManagerSystem _entityManager;
        private ResLoader _resLoader = ResLoader.Allocate();

        private void Awake()
        {
            _entityManager = this.GetSystem<IEntityManagerSystem>();

            this.GetSystem<INetworkSystem>().ReceiveEvent<EntityEnterResponse>(OnEntityEnterReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetSystem<INetworkSystem>().ReceiveEvent<EntityTransformSyncResponse>(OnEntitySyncReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEntityEnterReceived(EntityEnterResponse response)
        {
            foreach (var netEntity in response.Datas)
            {
                var entityId = netEntity.EntityId;
                var position = netEntity.Transform.Position.ToVector3();
                var rotation = Quaternion.Euler(netEntity.Transform.Direction.ToVector3());
                //TODO 根据Entity加载对应的Prefab
                _entityManager.SpawnEntity(_resLoader.LoadSync<EntityView>("HeroKnightMale Melee"), entityId, position,
                    rotation, false);
            }
        }

        private void OnEntitySyncReceived(EntityTransformSyncResponse response)
        {
            var entityId = response.EntityId;
            var position = response.Transform.Position.ToVector3();
            var rotation = Quaternion.Euler(response.Transform.Direction.ToVector3());
            var entity = _entityManager.GetEntityDict(false)[entityId];
            Debug.Assert(entity.EntityId == entityId);
            var data = new EntityTransformSyncData
            {
                Entity = entity,
                Position = position,
                Rotation = rotation,
                StateId = response.StateId,
                Data = response.Data.ToByteArray()
            };
            entity.HandleNetworkSync(data);
        }

        public IArchitecture GetArchitecture()
        {
            return GameApp.Interface;
        }

        void OnDestroy()
        {
            _resLoader.Recycle2Cache();
            _resLoader = null;
        }
    }
}
