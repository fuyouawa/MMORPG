using Common.Proto.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Entity
    {
        public EntityType EntityType;
        public int EntityId;
        public Map? Map;
        public Vector3 Position;
        public Vector3 Direction;
        public float ViewRange;

        public virtual void Update() { }
    }
}
