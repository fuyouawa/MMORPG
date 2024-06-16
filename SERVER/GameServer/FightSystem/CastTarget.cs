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
using GameServer.FightSystem;

namespace GameServer.FightSystem
{
    public abstract class CastTarget
    {
        public virtual Vector3 Position => Vector3.Zero;
        public virtual Vector3 Direction => Vector3.Zero;
        public virtual bool Selectable => false;
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
        public override bool Selectable
        {
            get
            {
                bool selectable = false;
                var actor = Entity as Actor;
                if (actor == null)
                {
                    return Entity.IsValid();
                }
                return !actor.IsDeath();
            }
        }
    }

    public class CastTargetPosition : CastTarget
    {
        private Vector3 _position;
        public override Vector3 Position => _position;

        public CastTargetPosition(Vector3 position)
        {
            _position = position;
        }

        public override bool Selectable => true;
    }
}
