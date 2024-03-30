using GameServer.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class TcpServer
    {
        private Acceptor _acceptor;
        private List<ClientSession> _clientSessions = new();

        public TcpServer(int port)
        {
            _acceptor = new(IPAddress.Parse("0.0.0.0"), port);
        }

        public async void Start()
        {
            while (true)
            {
                var socket = await _acceptor.AcceptAsync();
                Console.WriteLine($"客户端连接:{socket.RemoteEndPoint}");
                ClientSession session = new(socket);
                _clientSessions.Add(session);
            }
        }
    }
}
