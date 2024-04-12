using Common.Network;
using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Model
{
    public class Player
    {
        private NetChannel _channel;
        private int _playerId;
        private string _username;

        public Character? Character { get; set; }
        public NetChannel Channel { get { return _channel; } }
        public int PlayerId { get { return _playerId; } }
        public string Username { get { return _username; } }

        public Player(NetChannel channel, string username) 
        {
            _channel = channel;
            _username = username;
        }
    }
}
