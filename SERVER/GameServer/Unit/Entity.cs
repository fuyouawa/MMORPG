using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public enum EntityType
    {
        None = 0,
        Character = 1,
        Monster = 2,
        Npc = 3,
    }

    public class Entity
    {
        public EntityType EntityType;
        public int EntityId;
        public Vector3 Position;
        public Vector3 Direction;
        public float ViewRange;

        public virtual void Update() { }
    }
}
