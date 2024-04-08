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
using GameServer.Model;

namespace GameServer.Tool
{
    public static class ProtoHelper
    {
        public static NetVector3 ToNetVector3(this Vector3 vector3)
        {
            var netVector3 = new NetVector3()
            {
                X = vector3.X,
                Y = vector3.Y,
                Z = vector3.Z
            };
            return netVector3;
        }

        public static Vector3 ToVector3(this NetVector3 netVector3)
        {
            var vector3 = new Vector3()
            {
                X = netVector3.X,
                Y = netVector3.Y,
                Z = netVector3.Z
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
                CharacterId = character.CharacterId,
                Entity = character.ToNetEntity(),
                JobId = character.JobId,
                Name = character.Name,
                Level = character.Level,
                Exp = character.Exp,
                SpaceId = character.SpeedId,
                Gold = character.Gold,
                Hp = character.Hp,
                Mp = character.Mp,
            };
            return netCharacter;
        }
    }
}
