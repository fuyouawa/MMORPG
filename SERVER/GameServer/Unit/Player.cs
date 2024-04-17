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
        private NetChannel _channel;
        private int _playerId;
        private string _username;
        private Character? _character;

        public NetChannel Channel { get { return _channel; } }
        public int PlayerId { get { return _playerId; } }
        public string Username { get { return _username; } }

        public Character? Character { get { return _character; } }

        public Player(NetChannel channel, string username, int playerId) 
        {
            _channel = channel;
            _username = username;
            _playerId = playerId;
        }

        public void SetCharacter(Character character)
        {
            _character = character;
        }
    }
}
