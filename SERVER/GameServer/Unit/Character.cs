using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public class Character  : Actor
    {
        public Player Player;
        public int CharacterId;
        public int JobId;
        public int Exp;
        public int Gold;

        public override void Update()
        {
            
        }
    }
}
