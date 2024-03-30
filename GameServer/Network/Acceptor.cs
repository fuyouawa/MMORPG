using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    public class Acceptor
    {
        private Socket? _socket;
        private readonly IPEndPoint _ep;

        public Acceptor(IPAddress address, int port)
        {
            _ep = new(address, port);
        }

        public Task<Socket> AcceptAsync()
        {
            if (_socket == null)
            {
                _socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                _socket.Bind(_ep);
                _socket.Listen();
            }
            return _socket.AcceptAsync();
        }
    }
}
