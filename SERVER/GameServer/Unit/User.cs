using Common.Network;
using GameServer.Manager;
using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Unit
{
    public class User
    {
        public NetChannel Channel { get; }
        public int PlayerId { get; }
        public string Username { get; }

        public Player? Character { get; private set; }

        public User(NetChannel channel, string username, int playerId) 
        {
            Channel = channel;
            Username = username;
            PlayerId = playerId;
        }

        public void SetCharacter(Player character)
        {
            Character = character;
        }
    }
}
