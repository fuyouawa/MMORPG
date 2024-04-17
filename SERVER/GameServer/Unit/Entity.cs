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
    }
}
