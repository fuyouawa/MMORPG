using System.Diagnostics;
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
        public MissileManager MissileManager;
        public SpawnManager SpawnManager;
        public FightManager FightManager;

        private AoiWord _aoiWord;

        public Map(int mapId, string name)
        {
            MapId = mapId;
            Name = name;

            _aoiWord = new(20);

            PlayerManager = new(this);
            MonsterManager = new(this);
            MissileManager = new(this);
            SpawnManager = new(this);
            FightManager = new(this);
        }

        public void Start()
        {
            PlayerManager.Start();
            MonsterManager.Start();
            MissileManager.Start();
            SpawnManager.Start();
            FightManager.Start();
        }

        public void Update()
        {
            PlayerManager.Update();
            MonsterManager.Update();
            MissileManager.Update();
            SpawnManager.Update();
            FightManager.Update();
        }

        /// <summary>
        /// 广播实体进入场景
        /// </summary>
        public void EntityEnter(Entity entity)
        {
            Log.Information($"实体进入场景:{entity.EntityId}");

            lock (_aoiWord)
            {
                entity.AoiEntity = _aoiWord.Enter(entity.EntityId, entity.Position.X, entity.Position.Z);
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

            // 如果新实体是玩家
            // 向新玩家投递已在场景中的在其可视范围内的实体
            if (entity.EntityType == EntityType.Player)
            {
                res.Datas.Clear();
                var list = GetEntityFollowingList(entity);
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
                _aoiWord.Leave(entity.AoiEntity);
            }
        }

        /// <summary>
        /// 同步实体位置并向能观察到该实体的玩家广播消息
        /// </summary>
        /// <param name="entity"></param>
        public void EntityRefreshPosition(Entity entity)
        {
            var enterRes = new EntityEnterResponse();
            enterRes.Datas.Add(new EntityEnterData()
            {
                EntityId = entity.EntityId,
                UnitId = entity.UnitId,
                EntityType = entity.EntityType,
                Transform = ProtoHelper.ToNetTransform(entity.Position, entity.Direction),
            });

            var leaveRes = new EntityLeaveResponse();
            leaveRes.EntityIds.Add(entity.EntityId);

            bool init1 = false, init2 = false;

            lock (_aoiWord)
            {
                _aoiWord.Refresh(entity.AoiEntity, entity.Position.X, entity.Position.Z,
                    entityId =>
                    {
                        if (init1 == false)
                        {
                            enterRes.Datas.Clear();
                            init1 = true;
                        }
                        // 如果移动的是玩家，还需要向该玩家通知所有新加入视野范围的实体
                        Log.Debug($"[Map.EntityRefreshPosition]2.实体：{entityId} 进入了 实体：{entity.EntityId} 的视距范围");
                        if (entity.EntityType != EntityType.Player) return;
                        var enterEntity = EntityManager.Instance.GetEntity((int)entityId);
                        Debug.Assert(enterEntity != null);
                        enterRes.Datas.Add(new EntityEnterData()
                        {
                            EntityId = enterEntity.EntityId,
                            UnitId = enterEntity.UnitId,
                            EntityType = enterEntity.EntityType,
                            Transform = ProtoHelper.ToNetTransform(enterEntity.Position, enterEntity.Direction),
                        });
                    },
                    entityId =>
                    {
                        if (init2 == false)
                        {
                            leaveRes.EntityIds.Clear();
                            init2 = true;
                        }
                        // 如果移动的是玩家，还需要向该玩家通知所有退出其视野范围的实体
                        Log.Debug($"[Map.EntityRefreshPosition]2.实体：{entityId} 离开了 实体：{entity.EntityId} 的视距范围");
                        if (entity.EntityType != EntityType.Player) return;
                        var leaveEntity = EntityManager.Instance.GetEntity((int)entityId);
                        Debug.Assert(leaveEntity != null);
                        leaveRes.EntityIds.Add(leaveEntity.EntityId);
                    },
                    entityId =>
                    {
                        // 如果进入了一个玩家的视距范围，则向其通知有实体加入
                        Log.Debug($"[Map.EntityRefreshPosition]1.实体：{entity.EntityId} 进入了 实体：{entityId} 的视距范围");
                        var enterEntity = EntityManager.Instance.GetEntity((int)entityId);
                        Debug.Assert(enterEntity != null);
                        if (enterEntity.EntityType != EntityType.Player) return;

                        var player = enterEntity as Player;
                        Debug.Assert(player != null);
                        player?.User.Channel.Send(enterRes);
                    },
                    entityId =>
                    {
                        // 如果离开了一个玩家的视距范围，则向其通知有实体退出
                        Log.Debug($"[Map.EntityRefreshPosition]1.实体：{entity.EntityId} 离开了 实体：{entityId} 的视距范围");
                        var leaveEntity = EntityManager.Instance.GetEntity((int)entityId);
                        Debug.Assert(leaveEntity != null);
                        if (leaveEntity.EntityType != EntityType.Player) return;

                        var player = leaveEntity as Player;
                        Debug.Assert(player != null);
                        player?.User.Channel.Send(leaveRes);
                    });
            }

            if (entity.EntityType == EntityType.Player)
            {
                var player = entity as Player;
                Debug.Assert(player != null);
                if (init1)
                {
                    player?.User.Channel.Send(enterRes);
                }
                if (init2)
                {
                    player?.User.Channel.Send(leaveRes);
                }
            }

        }

        /// <summary>
        /// 获取指定实体视距范围内实体并按条件过滤
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<Entity> GetEntityFollowingList(Entity entity, Predicate<Entity>? condition = null)
        {
            var entityList = new List<Entity>();
            lock (_aoiWord)
            {
                _aoiWord.ScanFollowingList(entity.AoiEntity, followingEntityId =>
                {
                    var followingEntity = EntityManager.Instance.GetEntity((int)followingEntityId);
                    if (followingEntity != null && (condition == null || condition(followingEntity)))
                    {
                        entityList.Add(followingEntity);
                    }
                });
            }
            return entityList;
        }

        /// <summary>
        /// 获取指定实体视距范围内实体并按条件过滤
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public List<Entity> GetEntityFollowerList(Entity entity, Predicate<Entity>? condition = null)
        {
            var entityList = new List<Entity>();
            lock (_aoiWord)
            {
                _aoiWord.ScanFollowerList(entity.AoiEntity, followerEntityId =>
                {
                    var followerEntity = EntityManager.Instance.GetEntity((int)followerEntityId);
                    if (followerEntity != null && (condition == null || condition(followerEntity)))
                    {
                        entityList.Add(followerEntity);
                    }
                });
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
