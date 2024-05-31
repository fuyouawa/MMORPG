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
    public abstract class Target
    {
        protected object _realObj;
        
        public Target(object realObj)
        {
            _realObj = realObj;
        }

        public virtual object RealObj => _realObj;
        public virtual int Id => 0;
        public virtual Vector3 Position() => Vector3.Zero;
        public virtual Vector3 Direction() => Vector3.Zero;
    }

    public class EntityTarget : Target
    {
        private Entity _entity { get => (Entity)_realObj; }
        public EntityTarget(Entity entity) : base(entity) { }

        public override int Id => _entity.EntityId;
        public override Vector3 Position() => _entity.Position;
        public override Vector3 Direction() => _entity.Direction;

    }

    public class PositionTarget : Target
    {
        public PositionTarget(Vector3 position) : base(position) { }

        public override Vector3 Position()
        {
            return (Vector3)_realObj;
        }
    }
}
