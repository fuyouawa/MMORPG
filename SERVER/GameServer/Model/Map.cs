using Aoi;
using Common.Proto.Entity;
using Common.Proto.EventLike;
using Common.Proto.EventLike.Map;
using GameServer.Manager;
using GameServer.Tool;
using Serilog;
using Google.Protobuf;

namespace GameServer.Model
{
    public class Map
    {
        const int InvalidMapId = 0;

        public readonly int MapId;
        public readonly string Name;
        public readonly string Description = "";
        public readonly int Music = 0;

        public PlayerManager PlayerManager;
        public MonsterManager MonsterManager;
        public SpawnManager SpawnManager;

        private AoiWord _aoiWord;

        public Map(int mapId, string name)
        {
            MapId = mapId;
            Name = name;

            _aoiWord = new(20);

            PlayerManager = new(this);
            MonsterManager = new(this);
            SpawnManager = new(this);
        }

        public void Update()
        {
            PlayerManager.Update();
            MonsterManager.Update();
            SpawnManager.Update();
        }

        /// <summary>
        /// 广播实体进入场景
        /// </summary>
        public void EntityEnter(Entity entity)
        {
            Log.Information($"实体进入场景:{entity.EntityId}");

            entity.Map = this;

            //List<int> 
            lock (_aoiWord)
            {
                _aoiWord.Enter(entity.EntityId, entity.Position.X, entity.Position.Z, out var enters);
            }

            var res = new EntityEnterResponse();
            res.Datas.Add(new EntityEnterData()
            {
                EntityId = entity.EntityId,
                UnitId = entity.UnitId,
                EntityType = entity.EntityType,
                Transform = ProtoHelper.ToNetTransform(entity.Position, entity.Direction),
            });

            // 向能观察到新实体的角色广播新实体加入场景
            PlayerManager.Broadcast(res, entity);

            // 如果新实体是角色
            // 向新角色投递已在场景中的在其可视范围内的实体
            if (entity.EntityType == EntityType.Player)
            {
                res.Datas.Clear();
                var list = GetEntityViewEntityList(entity);
                foreach (var viewEntity in list)
                {
                    res.Datas.Add(new EntityEnterData()
                    {
                        EntityId = viewEntity.EntityId,
                        EntityType = viewEntity.EntityType,
                        UnitId = viewEntity.UnitId,
                        Transform = ProtoHelper.ToNetTransform(viewEntity.Position, viewEntity.Direction),
                    });
                }
                var currentPlayer = entity as Player;
                currentPlayer?.User.Channel.Send(res, null);
            }
        }

        /// <summary>
        ///  广播实体离开场景
        /// </summary>
        public void EntityLeave(Entity entity)
        {
            Log.Information($"实体离开场景:{entity.EntityId}");

            // 向能观察到实体的角色广播实体离开场景
            // 实际上直接广播是向当前entity的关注实体广播而非关注当前entity的实体
            // 如果所有实体的视野范围一致则没有这个问题，但如果不一致的话，需要考虑另行维护
            var res = new EntityLeaveResponse();
            res.EntityIds.Add(entity.EntityId);
            PlayerManager.Broadcast(res, entity);

            lock (_aoiWord)
            {
                _aoiWord.Leave(entity.EntityId, out var leaveList);
            }

            entity.Map = null;
        }

        /// <summary>
        /// 同步实体位置并向能观察到该实体的玩家广播消息
        /// </summary>
        /// <param name="entity"></param>
        public void EntityRefreshPosition(Entity entity)
        {
            List<int> enters;
            List<int> leaves;
            lock (_aoiWord)
            {
                _aoiWord.Refresh(entity.EntityId, entity.Position.X, entity.Position.Z, out enters, out leaves);
            }

            // 已假定所有实体的视距范围一致

            // 新进入当前实体范围内的实体如果是玩家，则向其通知有实体加入
            var enterRes = new EntityEnterResponse();
            enterRes.Datas.Add(new EntityEnterData()
            {
                EntityId = entity.EntityId,
                UnitId = entity.UnitId,
                EntityType = entity.EntityType,
                Transform = ProtoHelper.ToNetTransform(entity.Position, entity.Direction),
            });
            if (enters != null)
            {
                foreach (var entityId in enters)
                {
                    var enterEntity = EntityManager.Instance.GetEntity((int)entityId);
                    if (enterEntity.EntityType != EntityType.Player)
                    {
                        continue;
                    }

                    var player = enterEntity as Player;
                    player.User.Channel.Send(enterRes);
                }

                // 如果移动的是玩家，还需要向该玩家通知所有新加入视野范围的实体
                if (entity.EntityType == EntityType.Player && enters.Any())
                {
                    var player = entity as Player;
                    enterRes.Datas.Clear();
                    foreach (var entityId in enters)
                    {
                        var enterEntity = EntityManager.Instance.GetEntity((int)entityId);
                        enterRes.Datas.Add(new EntityEnterData()
                        {
                            EntityId = enterEntity.EntityId,
                            UnitId = enterEntity.UnitId,
                            EntityType = enterEntity.EntityType,
                            Transform = ProtoHelper.ToNetTransform(enterEntity.Position, enterEntity.Direction),
                        });
                    }

                    player.User.Channel.Send(enterRes);
                }
            }

            if (leaves != null)
            {
                // 退出当前实体范围内的实体如果是玩家，则向其通知有实体退出
                var leaveRes = new EntityLeaveResponse();
                leaveRes.EntityIds.Add(entity.EntityId);
                foreach (var entityId in leaves)
                {
                    var leaveEntity = EntityManager.Instance.GetEntity((int)entityId);
                    if (leaveEntity.EntityType != EntityType.Player)
                    {
                        continue;
                    }

                    var player = leaveEntity as Player;
                    player.User.Channel.Send(leaveRes);
                }

                // 如果移动的是玩家，还需要向该玩家通知所有退出视野范围的实体
                if (entity.EntityType == EntityType.Player && leaves.Any())
                {
                    var player = entity as Player;
                    leaveRes.EntityIds.Clear();
                    foreach (var entityId in leaves)
                    {
                        var leaveEntity = EntityManager.Instance.GetEntity((int)entityId);
                        leaveRes.EntityIds.Add(leaveEntity.EntityId);
                    }

                    player.User.Channel.Send(leaveRes);
                }
            }
        }

        /// <summary>
        /// 获取指定实体视距范围内实体并按条件过滤
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<Entity> GetEntityViewEntityList(Entity entity, Predicate<Entity>? condition = null)
        {
            var entityList = new List<Entity>();
            lock (_aoiWord)
            {
                var viewEntityIdSet = _aoiWord.GetViewEntityList(entity.EntityId);
                foreach (var viewEntityId in viewEntityIdSet)
                {
                    var viewEntity = EntityManager.Instance.GetEntity((int)viewEntityId);
                    if (viewEntity != null && (condition == null || condition(viewEntity)))
                    {
                        entityList.Add(viewEntity);
                    }
                }
            }
            return entityList;
        }

        /// <summary>
        /// 根据网络实体对象更新实体并广播新状态
        /// </summary>
        public void EntityTransformSync(int entityId, NetTransform transform, int stateId, ByteString data)
        {
            var entity = EntityManager.Instance.GetEntity(entityId);
            if (entity == null) return;

            entity.Position = transform.Position.ToVector3();
            entity.Direction = transform.Direction.ToVector3();
            EntityRefreshPosition(entity);

            var response = new EntityTransformSyncResponse
            {
                EntityId = entityId,
                Transform = transform,
                StateId = stateId,
                Data = data
            };

            // 向所有角色广播新实体的状态更新
            PlayerManager.Broadcast(response, entity);
        }

        /// <summary>
        /// 根据服务器实体对象更新实体并广播新状态
        /// </summary>
        public void EntitySync(int entityId, int stateId)
        {
            var entity = EntityManager.Instance.GetEntity(entityId);
            if (entity == null) return;
            EntityRefreshPosition(entity);

            var response = new EntityTransformSyncResponse
            {
                EntityId = entityId,
                Transform = new()
                {
                    Direction = entity.Position.ToNetVector3(),
                    Position = entity.Direction.ToNetVector3()
                },
                StateId = stateId,
                Data = null
            };

            // 向所有角色广播新实体的状态更新
            PlayerManager.Broadcast(response, entity);
        }

    }
}
