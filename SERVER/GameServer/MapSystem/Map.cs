using System.Diagnostics;
using Aoi;
using MMORPG.Common.Proto.Entity;
using MMORPG.Common.Proto.Map;
using GameServer.Tool;
using Serilog;
using Google.Protobuf;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Principal;
using GameServer.NpcSystem;
using GameServer.EntitySystem;
using GameServer.FightSystem;
using GameServer.InventorySystem;
using GameServer.PlayerSystem;
using GameServer.MissileSystem;
using GameServer.MonsterSystem;

namespace GameServer.MapSystem
{
    public class Map
    {
        const int InvalidMapId = 0;

        public MapDefine Define { get; }
        public int MapId => Define.ID;

        public PlayerManager PlayerManager { get; }
        public MonsterManager MonsterManager { get; }
        public NpcManager NpcManager { get; }
        public MissileManager MissileManager { get; }
        public SpawnManager SpawnManager { get; }
        public DroppedItemManager DroppedItemManager { get; }

        private AoiWord _aoiWord;

        public override string ToString()
        {
            return $"Map:\"{Define.Name}:{MapId}\"";
        }

        public Map(MapDefine mapDefine)
        {
            Define = mapDefine;

            _aoiWord = new(20);

            PlayerManager = new(this);
            MonsterManager = new(this);
            NpcManager = new(this);
            MissileManager = new(this);
            SpawnManager = new(this);
            DroppedItemManager = new(this);
        }

        public void Start()
        {
            PlayerManager.Start();
            MonsterManager.Start();
            NpcManager.Start();
            MissileManager.Start();
            SpawnManager.Start();
            DroppedItemManager.Start();
        }

        public void Update()
        {
            PlayerManager.Update();
            MonsterManager.Update();
            NpcManager.Update();
            MissileManager.Update();
            DroppedItemManager.Update();
            SpawnManager.Update();
        }

        /// <summary>
        /// 广播实体进入场景
        /// </summary>
        public void EntityEnter(Entity entity)
        {
            Log.Information($"{entity}进入{entity.Map}");

            entity.AoiEntity = _aoiWord.Enter(entity.EntityId, entity.Position.X, entity.Position.Y);
            
            var res = new EntityEnterResponse();
            res.Datas.Add(ConstructEntityEnterData(entity));

            // 向能观察到新实体的角色广播新实体加入场景
            PlayerManager.Broadcast(res, entity);

            // 如果新实体是玩家
            // 向新玩家投递已在场景中的在其可视范围内的实体
            if (entity.EntityType == EntityType.Player)
            {
                res.Datas.Clear();
                ScanEntityFollowing(entity, e =>
                {
                    res.Datas.Add(ConstructEntityEnterData(e));
                });
                var currentPlayer = entity as Player;
                currentPlayer?.User.Channel.Send(res);
            }
        }

        private EntityEnterData ConstructEntityEnterData(Entity entity)
        {
            var data = new EntityEnterData()
            {
                EntityId = entity.EntityId,
                UnitId = entity.UnitDefine.ID,
                EntityType = entity.EntityType,
                Transform = ProtoHelper.ToNetTransform(entity.Position, entity.Direction),
            };
            if (entity is Actor actor)
            {
                data.Actor = actor.ToNetActor();
            }
            return data;
        }

        /// <summary>
        ///  广播实体离开场景
        /// </summary>
        public void EntityLeave(Entity entity)
        {
            Log.Information($"{entity}离开{entity.Map}");

            // 向能观察到实体的角色广播实体离开场景
            // 实际上直接广播是向当前entity的关注实体广播而非关注当前entity的实体
            // 如果所有实体的视野范围一致则没有这个问题，但如果不一致的话，需要考虑另行维护
            var res = new EntityLeaveResponse();
            res.EntityIds.Add(entity.EntityId);
            PlayerManager.Broadcast(res, entity);
            _aoiWord.Leave(entity.AoiEntity);
        }

        /// <summary>
        /// 同步实体位置并向能观察到该实体的玩家广播消息
        /// </summary>
        /// <param name="entity"></param>
        public void EntityRefreshPosition(Entity entity)
        {
            var enterRes = new EntityEnterResponse();
            enterRes.Datas.Add(ConstructEntityEnterData(entity));

            var leaveRes = new EntityLeaveResponse();
            leaveRes.EntityIds.Add(entity.EntityId);

            bool init1 = false, init2 = false;

            _aoiWord.Refresh(entity.AoiEntity, entity.Position.X, entity.Position.Y,
                entityId =>
                {
                    if (init1 == false)
                    {
                        enterRes.Datas.Clear();
                        init1 = true;
                    }
                    // 如果移动的是玩家，还需要向该玩家通知所有新加入视野范围的实体
                    // Log.Debug($"[Map.EntityRefreshPosition]2.实体：{entityId} 进入了 实体：{entity.EntityId} 的视距范围");
                    if (entity.EntityType != EntityType.Player) return;
                    var enterEntity = EntityManager.Instance.GetEntity(entityId);
                    Debug.Assert(enterEntity != null);
                    enterRes.Datas.Add(ConstructEntityEnterData(enterEntity));
                },
                entityId =>
                {
                    if (init2 == false)
                    {
                        leaveRes.EntityIds.Clear();
                        init2 = true;
                    }
                    // 如果移动的是玩家，还需要向该玩家通知所有退出其视野范围的实体
                    // Log.Debug($"[Map.EntityRefreshPosition]2.实体：{entityId} 离开了 实体：{entity.EntityId} 的视距范围");
                    if (entity.EntityType != EntityType.Player) return;
                    var leaveEntity = EntityManager.Instance.GetEntity(entityId);
                    Debug.Assert(leaveEntity != null);
                    leaveRes.EntityIds.Add(leaveEntity.EntityId);
                },
                entityId =>
                {
                    // 如果进入了一个玩家的视距范围，则向其通知有实体加入
                    // Log.Debug($"[Map.EntityRefreshPosition]1.实体：{entity.EntityId} 进入了 实体：{entityId} 的视距范围");
                    var enterEntity = EntityManager.Instance.GetEntity(entityId);
                    Debug.Assert(enterEntity != null);
                    if (enterEntity.EntityType != EntityType.Player) return;

                    var player = enterEntity as Player;
                    Debug.Assert(player != null);
                    player?.User.Channel.Send(enterRes);
                },
                entityId =>
                {
                    // 如果离开了一个玩家的视距范围，则向其通知有实体退出
                    // Log.Debug($"[Map.EntityRefreshPosition]1.实体：{entity.EntityId} 离开了 实体：{entityId} 的视距范围");
                    var leaveEntity = EntityManager.Instance.GetEntity(entityId);
                    Debug.Assert(leaveEntity != null);
                    if (leaveEntity.EntityType != EntityType.Player) return;

                    var player = leaveEntity as Player;
                    Debug.Assert(player != null);
                    player?.User.Channel.Send(leaveRes);
                });
        
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
        /// 获取指定实体视距范围内实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public void ScanEntityFollowing(Entity entity, Action<Entity> callback)
        {
            _aoiWord.ScanFollowingList(entity.AoiEntity, followingEntityId =>
            {
                var followingEntity = EntityManager.Instance.GetEntity(followingEntityId);
                if (followingEntity != null) callback(followingEntity);
            });
        }

        /// <summary>
        /// 按半径获取指定实体视距范围内实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public void ScanEntityFollowing(Entity entity, float range, Action<Entity> callback)
        {
        }


        /// <summary>
        /// 获取指定实体视距范围内实体
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public void ScanEntityFollower(Entity entity, Action<Entity> callback)
        {
            _aoiWord.ScanFollowerList(entity.AoiEntity, followerEntityId =>
            {
                var followerEntity = EntityManager.Instance.GetEntity(followerEntityId);
                if (followerEntity != null) callback(followerEntity);
            });
        }

        public Entity? GetEntityFollowingNearest(Entity entity, Predicate<Entity>? condition = null)
        {
            Entity? nearest = null;
            float minDistance = 0;
            _aoiWord.ScanFollowerList(entity.AoiEntity, followerEntityId =>
            {
                var followerEntity = EntityManager.Instance.GetEntity(followerEntityId);
                if (followerEntity != null && (condition == null || condition(followerEntity)))
                {
                    if (nearest == null)
                    {
                        nearest = followerEntity;
                        minDistance = Vector2.Distance(followerEntity.Position, entity.Position);
                    }
                    else
                    {
                        var tmp = Vector2.Distance(followerEntity.Position, entity.Position);
                        if (tmp < minDistance)
                        {
                            nearest = followerEntity;
                            minDistance = tmp;
                        }
                    }
                }
            });
            return nearest;
        }

        /// <summary>
        /// 根据网络实体对象更新实体并广播新状态
        /// </summary>
        public void EntityTransformSync(int entityId, NetTransform transform, int stateId, ByteString data)
        {
            var entity = EntityManager.Instance.GetEntity(entityId);
            if (entity == null) return;

            entity.Position = transform.Position.ToVector3().ToVector2();
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
                    Direction = entity.Position.ToVector3().ToNetVector3(),
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
