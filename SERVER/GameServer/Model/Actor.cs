using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Actor : Entity
    {
        public string Name;
        public int Level;
        public int SpeedId;
        public int Hp;
        public int Mp;
        public int Gold;
    }
}
