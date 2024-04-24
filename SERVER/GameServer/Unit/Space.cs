using AOI;
using Common.Proto.Entity;
using Common.Proto.Event.Space;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public class Space
    {
        const int InvalidSpaceId = 0;

        public readonly int SpaceId;
        public readonly string Name;
        public readonly string Description = "";
        public readonly int Music = 0;

        public CharacterManager CharacterManager;
        public MonsterManager MonsterManager;

        private AoiZone _aoiZone;

        public Space(int spaceId, string name)
        {
            SpaceId = spaceId;
            Name = name;

            CharacterManager = new(this);
            MonsterManager = new(this);

            _aoiZone = new(.001f, .001f);
        }

        /// <summary>
        /// 广播实体进入场景
        /// </summary>
        public void EntityEnter(Entity entity)
        {
            Log.Information($"实体进入场景:{entity.EntityId}");

            lock (_aoiZone)
            {
                Vector2 range = new (entity.ViewRange, entity.ViewRange);
                _aoiZone.Enter(entity.EntityId, entity.Position.X, entity.Position.Z, range, out var enters);
            }

            var res = new EntityEnterResponse();
            res.EntityList.Add(entity.ToNetEntity());

            // 向能观察到新实体的角色广播新实体加入场景
            CharacterManager.Broadcast(res, entity);

            // 如果新实体是角色
            // 向新角色投递已在场景中的在其可视范围内的实体
            if (entity.EntityType == EntityType.Character)
            {
                res.EntityList.Clear();
                var list = GetEntityViewEntityList(entity);
                foreach (var viewEntity in list)
                {
                    res.EntityList.Add(viewEntity.ToNetEntity());
                }
                var currentCharacter = entity as Character;
                currentCharacter?.Player.Channel.Send(res, null);
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
            // 这点是还没处理的，未来考虑维护一下实体与关注该实体的角色
            var res = new EntityLeaveResponse();
            res.EntityId = entity.EntityId;
            CharacterManager.Broadcast(res, entity);

            lock (_aoiZone)
            {
                _aoiZone.Exit(entity.EntityId);
            }
        }

        public void EntityRefreshPosition(Entity entity)
        {
            lock (_aoiZone)
            {
                var aoiEntity = _aoiZone.Refresh(entity.EntityId, entity.Position.ToVector2());
                foreach (var leaveEntity in aoiEntity.Leave)
                {
                    
                }
            }
        }

        public List<Entity> GetEntityViewEntityList(Entity entity, Predicate<Entity>? condition = null)
        {
            var entityList = new List<Entity>();
            lock (_aoiZone)
            {
                var viewEntityIdSet = _aoiZone[entity.EntityId].ViewEntity;
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
        /// 根据网络实体对象更新实体并广播
        /// </summary>
        /// <param name="netEntity"></param>
        public void EntityUpdate(NetEntity netEntity)
        {
            var entity = EntityManager.Instance.GetEntity(netEntity.EntityId);
            if (entity == null) return;

            entity.Position = netEntity.Position.ToVector3();
            entity.Direction = netEntity.Direction.ToVector3();
            EntityRefreshPosition(entity);

            var res = new EntitySyncResponse() { EntitySync = new() };
            // res.EntitySync.Status = ;
            res.EntitySync.Entity = netEntity;

            // 向所有角色广播新实体的状态更新
            CharacterManager.Broadcast(res, entity);
        }
    }
}
