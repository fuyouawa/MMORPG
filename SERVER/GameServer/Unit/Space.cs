using AOI;
using Common.Proto.Entity;
using Common.Proto.Space;
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
            var res = new EntityEnterResponse();
            res.EntityList.Add(entity.ToNetEntity());

            lock (_aoiZone)
            {
                _aoiZone.Enter(entity.EntityId, entity.Position.X, entity.Position.Z);
            }

            // 向所有角色广播新实体加入场景
            CharacterManager.Broadcast(res, entity);

            // 如果新实体是角色，
            // 向新角色投递已在场景中的所有实体
            if (entity.EntityType == EntityType.Character)
            {
                res.EntityList.Clear();
                var list = GetEntityViewEntityList(entity);
                foreach (var viewEntity in list)
                {
                    res.EntityList.Add(viewEntity.ToNetEntity());
                }
                var currentCharacter = entity as Character;
                currentCharacter.Player.Channel.Send(res, null);
            }
        }

        /// <summary>
        ///  广播实体离开场景
        /// </summary>
        public void EntityLeave(Entity entity)
        {
            Log.Information($"实体离开场景:{entity.EntityId}");

            var res = new EntityLeaveResponse();
            res.EntityId = entity.EntityId;

            lock (_aoiZone)
            {
                _aoiZone.Exit(entity.EntityId);
            }

            // 向所有角色广播新实体离开场景
            CharacterManager.Broadcast(res, entity);
        }

        public void EntityMove(Entity entity, Vector3 newPos)
        {
            entity.Position = newPos;
            lock (_aoiZone)
            {
                _aoiZone.Refresh(entity.EntityId, newPos.ToVector2());
            }
        }

        public List<Entity> GetEntityViewEntityList(Entity entity, EntityType type = EntityType.None)
        {
            var entityList = new List<Entity>();
            lock (_aoiZone)
            {
                var viewEntityIdSet = _aoiZone[entity.EntityId].ViewEntity;
                foreach (var viewEntityId in viewEntityIdSet)
                {
                    var viewEntity = EntityManager.Instance.GetEntity((int)viewEntityId);
                    if (viewEntity != null && (type == EntityType.None || viewEntity.EntityType == type))
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
            Entity? entity = EntityManager.Instance.GetEntity(netEntity.EntityId);
            if (entity == null) return;

            EntityMove(entity, netEntity.Position.ToVector3());
            entity.Direction = netEntity.Direction.ToVector3();

            var res = new EntitySyncResponse() { EntitySync = new() };
            // res.EntitySync.Status = ;
            res.EntitySync.Entity = netEntity;

            // 向所有角色广播新实体的状态更新
            CharacterManager.Broadcast(res, entity);
        }
    }
}
