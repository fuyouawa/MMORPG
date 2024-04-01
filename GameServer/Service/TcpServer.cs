using Common.Network;
using GameServer.Network;
using GameServer.Tool;
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
        private List<Session> _clientSessions = new();

        public TcpServer(int port)
        {
            _acceptor = new(IPAddress.Parse("0.0.0.0"), port);
        }

        public void Start()
        {
            Global.Logger.Info("开启服务器");
            Task.Run(async () =>
            {
                while (true)
                {
                    var socket = await _acceptor.AcceptAsync();
                    Global.Logger.Info($"客户端连接:{socket.RemoteEndPoint}");
                    Session session = new(socket);
                    session.Start();
                    _clientSessions.Add(session);
                }
            });
        }
    }
}
