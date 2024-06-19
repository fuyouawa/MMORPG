using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Map;
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
        private IDataManagerSystem _dataManager;
        private INetworkSystem _network;
        private ResLoader _resLoader = ResLoader.Allocate();

        private void Awake()
        {
            _entityManager = this.GetSystem<IEntityManagerSystem>();
            _dataManager = this.GetSystem<IDataManagerSystem>();
            _network = this.GetSystem<INetworkSystem>();

            _network.Receive<EntityEnterResponse>(OnEntityEnterReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _network.Receive<EntityLeaveResponse>(OnEntityLeaveReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _network.Receive<EntityTransformSyncResponse>(OnEntitySyncReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
            _network.Receive<EntityHurtResponse>(OnEntityHurtReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEntityHurtReceived(EntityHurtResponse response)
        {
            foreach (var damage in response.Damages)
            {
                
            }
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
                
                var unitDefine = _dataManager.GetUnitDefine(data.UnitId);

                _entityManager.SpawnEntity(
                    _resLoader.LoadSync<EntityView>(unitDefine.Resource),
                    entityId,
                    data.UnitId,
                    data.EntityType,
                    position,
                    rotation);
            }
        }

        private void OnEntitySyncReceived(EntityTransformSyncResponse response)
        {
            var entityId = response.EntityId;
            var position = response.Transform.Position.ToVector3();
            var rotation = Quaternion.Euler(response.Transform.Direction.ToVector3());
            var entity = _entityManager.GetEntityById(entityId);
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
