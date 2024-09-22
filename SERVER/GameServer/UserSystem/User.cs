using MMORPG.Common.Network;
using GameServer.Manager;
using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GameServer.Db;
using GameServer.NpcSystem;
using GameServer.PlayerSystem;

namespace GameServer.UserSystem
{
    public class User
    {
        public NetChannel Channel { get; }
        public long UserId => DbUser.Id;
        public DbUser DbUser { get; }

        public Player? Player { get; private set; }

        public User(NetChannel channel, DbUser dbUser)
        {
            Channel = channel;
            DbUser = dbUser;
        }

        public override string ToString()
        {
            return $"User:{DbUser.Username}({UserId})";
        }

        public void Start() { }

        public void Update() { }

        public void SetPlayer(Player player)
        {
            Player = player;
        }
    }
}
