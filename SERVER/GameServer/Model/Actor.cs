using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public enum ActorState
    {
        Idle = 1,
        Move = 2,
    }

    public class Actor : Entity
    {
        public ActorState State;
        public string Name;
        public int Speed;
        public int Level;
        public int Hp;
        public int Mp;

        public Actor(Map map, string name)
        {
            Map = map;
            Name = name;
        }

        public bool IsDeath()
        {
            return Hp <= 0;
        }
    }
}
