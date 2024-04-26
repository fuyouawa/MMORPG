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
    public class Player
    {
        public NetChannel Channel { get; }
        public int PlayerId { get; }
        public string Username { get; }

        public Character? Character { get; private set; }

        public Player(NetChannel channel, string username, int playerId) 
        {
            Channel = channel;
            Username = username;
            PlayerId = playerId;
        }

        public void SetCharacter(Character character)
        {
            Character = character;
        }
    }
}
