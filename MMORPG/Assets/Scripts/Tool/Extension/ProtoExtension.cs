using Common.Proto.Base;
using Common.Proto.Entity;
using Common.Proto.Player;
using UnityEngine;

namespace Tool
{
    public static class ProtoExtension
    {
        public static NetVector3 ToNetVector3(this Vector3 vector3)
        {
            var netVector3 = new NetVector3()
            {
                X = (int)(vector3.x * 1000),
                Y = (int)(vector3.y * 1000),
                Z = (int)(vector3.z * 1000)
            };
            return netVector3;
        }

        public static Vector3 ToVector3(this NetVector3 netVector3)
        {
            var vector3 = new Vector3()
            {
                x = (float)(netVector3.X) / 1000,
                y = (float)(netVector3.Y) / 1000,
                z = (float)(netVector3.Z) / 1000
            };
            return vector3;
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

        //public static NetCharacter ToNetCharacter(this Character character)
        //{
        //    var netCharacter = new NetCharacter()
        //    {
        //        CharacterId = character.CharacterId,
        //        Entity = character.ToNetEntity(),
        //        JobId = character.JobId,
        //        Name = character.Name,
        //        Level = character.Level,
        //        Exp = character.Exp,
        //        SpaceId = character.SpeedId,
        //        Gold = character.Gold,
        //        Hp = character.Hp,
        //        Mp = character.Mp,
        //    };
        //    return netCharacter;
        //}
    }
}
