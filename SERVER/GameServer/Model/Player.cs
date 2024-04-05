using Common.Network;
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
        private Connection _connection;

        public Character Character {  get { return _character; } }
        public Connection Connection { get { return _connection;} }
    }
}
