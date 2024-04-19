using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Numerics;
using System.Runtime.Intrinsics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Base;
using Common.Proto.Entity;
using Common.Proto.Player;
using GameServer.Unit;

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

        public static NetEntity ToNetEntity(this Entity entity)
        {
            var netEntity = new NetEntity()
            {
                EntityId = entity.EntityId,
                Direction = entity.Direction.ToNetVector3(),
                Position = entity.Position.ToNetVector3(),
            };
            return netEntity;
        }

        public static Entity ToEntity(this NetEntity netEntity)
        {
            var entity = new Entity()
            {
                EntityId = netEntity.EntityId,
                Direction = netEntity.Direction.ToVector3(),
                Position = netEntity.Position.ToVector3(),
            };
            return entity;
        }

        public static NetCharacter ToNetCharacter(this Character character)
        {
            var netCharacter = new NetCharacter()
            {
                Entity = character.ToNetEntity(),
                Name = character.Name,
                Level = character.Level,
                SpaceId = character.Space.SpaceId,
                Hp = character.Hp,
                Mp = character.Mp,

                CharacterId = character.CharacterId,
                JobId = character.JobId,
                Exp = character.Exp,
                Gold = character.Gold,
            };
            return netCharacter;
        }
    }
}
