using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Proto.Fight;
using GameServer.Manager;
using GameServer.EntitySystem;

namespace GameServer.FightSystem
{
    public abstract class CastTarget
    {
        public virtual Vector3 Position => Vector3.Zero;
        public virtual Vector3 Direction => Vector3.Zero;
    }

    public class CastTargetEntity : CastTarget
    {
        public Entity Entity;

        public CastTargetEntity(Entity entity)
        {
            Entity = entity;
        }

        public override Vector3 Position => Entity.Position;
        public override Vector3 Direction => Entity.Direction;
    }

    public class CastTargetPosition : CastTarget
    {
        private Vector3 _position;
        public override Vector3 Position => _position;

        public CastTargetPosition(Vector3 position)
        {
            _position = position;
        }

    }
}
