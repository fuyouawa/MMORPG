using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Character  : Actor
    {
        public int CharacterId;
        public int JobId;
        public int Exp;
        public int Hp;
        public int Mp;
    }
}
