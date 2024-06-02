using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Proto.Entity;

namespace GameServer.Model
{
    public class Player : Actor
    {
        //public static readonly float DefaultViewRange = 100;

        public User User;
        public int CharacterId;
        public int JobId;
        public int Exp;
        public int Gold;

        public Player(int entityId, int unitId, 
            Map map, string characterName, User user) 
            : base(EntityType.Player, entityId, unitId, map, characterName)
        {
            User = user;
        }

        public override void Start()
        {
            base.Start();
        }

        public override void Update()
        {
            base.Update();
        }
    }
}
