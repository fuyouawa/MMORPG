using Common.Network;
using Common.Proto;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class TcpServer
    {
        private Socket _serverSocket;
        private List<Session> _clientSessions = new();

        public TcpServer(int port)
        {
            _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
        }

        public async Task Run()
        {
            Global.Logger.Info("[Server] 开启服务器");
            _serverSocket.Listen();
            while (true)
            {
                var socket = await _serverSocket.AcceptAsync();
                Global.Logger.Info($"[Server] 客户端连接:{socket.RemoteEndPoint}");
                Session session = new(socket);
                session.PacketReceived += OnPacketReceived;
                Task.Run(session.StartAsync);
                _clientSessions.Add(session);
            }
        }

        private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            MessageRouter.Instance.DispatchMessage(sender, NetMessage.Parser.ParseFrom(e.Packet.Data));
        }
    }
}
