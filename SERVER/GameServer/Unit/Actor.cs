using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public class Actor : Entity
    {
        public Space? Space;
        public string Name;
        public int Speed;
        public int Level;
        public int Hp;
        public int Mp;
    }
}
