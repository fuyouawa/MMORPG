using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Fight;
using GameServer.Manager;
using GameServer.Model;

namespace GameServer.Fight
{
    public abstract class CastTarget
    {
        protected object _realObj;
        
        public CastTarget(object realObj)
        {
            _realObj = realObj;
        }

        public virtual object RealObj => _realObj;
        public virtual int Id => 0;
        public virtual Vector3 Position() => Vector3.Zero;
        public virtual Vector3 Direction() => Vector3.Zero;
    }

    public class EntityCastTarget : CastTarget
    {
        private Entity _entity => (Entity)_realObj;
        public EntityCastTarget(Entity entity) : base(entity) { }

        public override int Id => _entity.EntityId;
        public override Vector3 Position() => _entity.Position;
        public override Vector3 Direction() => _entity.Direction;
    }

    public class PositionCastTarget : CastTarget
    {
        public PositionCastTarget(Vector3 position) : base(position) { }

        public override Vector3 Position()
        {
            return (Vector3)_realObj;
        }
    }
}
