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
        private Character _character;
        private NetChannel _channel;

        public Character Character { get { return _character; } }
        public NetChannel Channel { get { return _channel; } }


        public Player(NetChannel channel, Character character) 
        {
            _channel = channel;
            _character = character;
        }
    }
}
