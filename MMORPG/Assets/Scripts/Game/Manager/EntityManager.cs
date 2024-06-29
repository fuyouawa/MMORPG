using System;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Fight;
using MMORPG.Common.Proto.Map;
using MMORPG.Event;
using QFramework;
using MMORPG.System;
using MMORPG.Tool;
using Serilog;
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
            _network.Receive<EntityAttributeSyncResponse>(OnEntityAttributeSyncReceived)
                .UnRegisterWhenGameObjectDestroyed(gameObject);
        }

        private void OnEntityAttributeSyncReceived(EntityAttributeSyncResponse response)
        {
            if (_entityManager.EntityDict.TryGetValue(response.EntityId, out var entity))
            {
                var actor = entity.GetComponent<ActorController>();
                foreach (var entry in response.Entrys)
                {
                    Log.Debug($"{actor.gameObject.name}属性同步:{entry.Type}");
                    switch (entry.Type)
                    {
                        case EntityAttributeEntryType.Level:
                            actor.Level.Value = entry.Int32;
                            break;
                        case EntityAttributeEntryType.Gold:
                            actor.Gold = entry.Int32;
                            break;
                        case EntityAttributeEntryType.Hp:
                            actor.Hp = entry.Int32;
                            break;
                        case EntityAttributeEntryType.Mp:
                            actor.Mp = entry.Int32;
                            break;
                        case EntityAttributeEntryType.Exp:
                            actor.Exp = entry.Int32;
                            break;
                        case EntityAttributeEntryType.MaxHp:
                            actor.MaxHp = entry.Int32;
                            break;
                        case EntityAttributeEntryType.MaxExp:
                            actor.MaxExp = entry.Int32;
                            break;
                        case EntityAttributeEntryType.MaxMp:
                            actor.MaxMp = entry.Int32;
                            break;
                        case EntityAttributeEntryType.FlagState:
                            actor.FlagState.Value = (FlagStates)entry.Int32;
                            break;
                        case EntityAttributeEntryType.None:
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        private void OnEntityHurtReceived(EntityHurtResponse response)
        {
            if (_entityManager.EntityDict.TryGetValue(response.Info.TargetId, out var wounded))
            {
                if (_entityManager.EntityDict.TryGetValue(response.Info.AttackerInfo.AttackerId, out var attacker))
                {
                    attacker.OnHit?.Invoke(response.Info);
                    Log.Information($"'{wounded.gameObject.name}'受到'{attacker.gameObject.name}'的{response.Info.AttackerInfo.AttackerType}攻击, 扣除{response.Info.Amount}点血量");
                }
                else
                {
                    Log.Information($"'{wounded.gameObject.name}'受到EntityId:{response.Info.AttackerInfo.AttackerId}(已离开视野范围)的{response.Info.AttackerInfo.AttackerType}攻击({response.Info.DamageType}), 扣除{response.Info.Amount}点血量");
                }

                wounded.OnHurt?.Invoke(response.Info);

                this.SendEvent(new EntityHurtEvent(
                    wounded,
                    attacker,
                    response.Info));
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

                var entity = _entityManager.SpawnEntity(
                    _resLoader.LoadSync<EntityView>(unitDefine.Resource),
                    entityId,
                    data.UnitId,
                    data.EntityType,
                    position,
                    rotation);

                if (entity.TryGetComponent<ActorController>(out var actor))
                {
                    actor.ApplyNetActor(data.Actor, true);
                }
            }
        }

        private void OnEntitySyncReceived(EntityTransformSyncResponse response)
        {
            if (_entityManager.EntityDict.TryGetValue(response.EntityId, out var entity))
            {
                var position = response.Transform.Position.ToVector3();
                var rotation = Quaternion.Euler(response.Transform.Direction.ToVector3());
                Debug.Assert(entity.EntityId == response.EntityId);
                var data = new EntityTransformSyncData
                {
                    Entity = entity,
                    Position = position,
                    Rotation = rotation,
                    StateId = response.StateId,
                    Data = response.Data.ToByteArray()
                };
                entity.OnTransformSync?.Invoke(data);
            }
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
