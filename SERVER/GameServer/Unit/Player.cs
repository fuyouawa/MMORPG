using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public class Player : Actor
    {
        public User User;
        public int CharacterId;
        public int JobId;
        public int Exp;
        public int Gold;

        public Player(Map map, string characterName, User user) : base(map, characterName)
        {
            User = user;
        }

        public override void Update()
        {
            
        }

        public bool IsOnline()
        {
            return true;
        }
    }
}
