using Common.Proto.Event;
using Common.Proto.Event.Map;
using QFramework;
using MMORPG.System;
using MMORPG.Tool;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

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

            this.GetSystem<INetworkSystem>().ReceiveEventInUnityThread<EntityEnterResponse>(OnEntityEnterReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetSystem<INetworkSystem>().ReceiveEventInUnityThread<EntityLeaveResponse>(OnEntityLeaveReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            this.GetSystem<INetworkSystem>().ReceiveEventInUnityThread<EntityTransformSyncResponse>(OnEntitySyncReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEntityLeaveReceived(EntityLeaveResponse response)
        {
            foreach (var id in response.EntityIds)
            {
                _entityManager.LeaveEntity(id);
            }
        }

        private void OnEntityEnterReceived(EntityEnterResponse response)
        {
            foreach (var data in response.Datas)
            {
                var entityId = data.EntityId;
                var position = data.Transform.Position.ToVector3();
                var rotation = Quaternion.Euler(data.Transform.Direction.ToVector3());
                
                var dataManager = this.GetSystem<IDataManagerSystem>();
                var unitDefine = dataManager.GetUnitDefine(data.UnitId);

                _entityManager.SpawnEntity(
                    _resLoader.LoadSync<EntityView>(unitDefine.Resource),
                    entityId,
                    (EntityType)data.EntityType,
                    false,
                    position,
                    rotation);
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
