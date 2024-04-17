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

        public Space(int spaceId, string name)
        {
            SpaceId = spaceId;
            Name = name;

            CharacterManager = new(this);
            MonsterManager = new(this);
        }

        /// <summary>
        /// 广播实体进入场景
        /// </summary>
        public void EntityEnter(Entity entity)
        {
            Log.Information($"实体进入场景:{entity.EntityId}");
            var res = new EntityEnterResponse();
            res.EntityList.Add(entity.ToNetEntity());

            // 向所有角色广播新实体加入场景
            CharacterManager.Broadcast(res, entity);

            // 如果新实体是角色，
            // 向新角色投递已在场景中的所有玩家
            if (entity.EntityType == EntityType.Character)
            {
                CharacterManager.CharacterListToNetEntityList(res.EntityList, entity);
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

            // 向所有角色广播新实体离开场景
            CharacterManager.Broadcast(res, entity);
        }

        /// <summary>
        /// 根据网络实体对象更新实体并广播
        /// </summary>
        /// <param name="netEntity"></param>
        public void EntityUpdate(NetEntity netEntity)
        {
            Entity? entity = EntityManager.Instance.GetEntity(netEntity.EntityId);
            if (entity == null) return;

            entity.Position = netEntity.Position.ToVector3();
            entity.Direction = netEntity.Direction.ToVector3();

            var res = new EntitySyncResponse() { EntitySync = new() };
            // res.EntitySync.Status = ;
            res.EntitySync.Entity = netEntity;

            // 向所有角色广播新实体的状态更新
            CharacterManager.Broadcast(res, entity);
        }
    }
}
