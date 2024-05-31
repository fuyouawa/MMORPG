using Common.Network;
using GameServer.Manager;
using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class User
    {
        public NetChannel Channel { get; }
        public long UserId { get; }
        public string Username { get; }

        public Player? Player { get; private set; }

        public User(NetChannel channel, string username, long userId) 
        {
            Channel = channel;
            Username = username;
            UserId = userId;
        }

        public void Start() { }

        public void Update() { }

        public void SetPlayer(Player player)
        {
            Player = player;
        }
    }
}
