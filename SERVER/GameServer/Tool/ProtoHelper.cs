using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Base;
using MMORPG.Common.Proto.Character;
using MMORPG.Common.Proto.Entity;
using GameServer.Db;
using GameServer.EntitySystem;
using GameServer.MonsterSystem;
using GameServer.PlayerSystem;

namespace GameServer.Tool
{
    public static class ProtoHelper
    {
        public static NetVector3 ToNetVector3(this Vector3 vector3)
        {
            var netVector3 = new NetVector3()
            {
                X = (int)(vector3.X * 1000),
                Y = (int)(vector3.Y * 1000),
                Z = (int)(vector3.Z * 1000)
            };
            return netVector3;
        }

        public static Vector3 ToVector3(this NetVector3 netVector3)
        {
            var vector3 = new Vector3()
            {
                X = netVector3.X * 0.001f,
                Y = netVector3.Y * 0.001f,
                Z = netVector3.Z * 0.001f,
            };
            return vector3;
        }

        public static NetTransform ToNetTransform(Vector3 pos, Vector3 dire)
        {
            var transform = new NetTransform();
            transform.Position = pos.ToNetVector3();
            transform.Direction = dire.ToNetVector3();
            return transform;
        }

        public static NetTransform ToNetTransform(Vector2 pos, Vector3 dire)
        {
            return ToNetTransform(pos.ToVector3(), dire);
        }

        public static NetActor ToNetActor(this Actor actor)
        {
            var netActor = new NetActor()
            {
                FlagState = actor.FlagState,
                Level = (int)actor.Level,
                MaxHp = (int)actor.AttributeManager.Final.MaxHp,
                Hp = (int)actor.Hp,
                MaxMp = (int)actor.AttributeManager.Final.MaxMp,
                Mp = (int)actor.Mp,
            };
            if (actor is Player player)
            {
                netActor.Exp = player.Exp;
                netActor.MaxExp = Player.CalculateExp(player.Level);
            }else if (actor is Monster monster)
            {
                netActor.ResurrectionTime = monster.SpawnDefine.Period;
            }
            return netActor;
        }

        //public static NetEntity ToNetEntity(this Entity entity)
        //{
        //    var netEntity = new NetEntity()
        //    {
        //        EntityId = entity.EntityId,
        //        Direction = entity.Direction.ToNetVector3(),
        //        Position = entity.Position.ToNetVector3(),
        //    };
        //    return netEntity;
        //}

        //public static Entity ToEntity(this NetEntity netEntity)
        //{
        //    var entity = new Entity()
        //    {
        //        EntityId = netEntity.EntityId,
        //        Direction = netEntity.Direction.ToVector3(),
        //        Position = netEntity.Position.ToVector3(),
        //    };
        //    return entity;
        //}

        public static NetCharacter ToNetCharacter(this DbCharacter character)
        {
            var netCharacter = new NetCharacter()
            {
                CharacterId = character.Id,
                UnitId = character.UnitId,
                Name = character.Name,
                MapId = character.MapId,
                Level = character.Level,
                Exp = character.Exp,
                Gold = character.Gold,
            };
            return netCharacter;
        }

        

    }
}
