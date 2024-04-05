using Common.Network;
using Common.Proto;
using Common.Proto.User;
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
        private List<NetChannel> _channels = new();

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
                NetChannel channel = new(socket);
                channel.PacketReceived += OnPacketReceived;
                Task.Run(channel.StartAsync);
                _channels.Add(channel);
            }
        }

        private void OnPacketReceived(Connection sender, PacketReceivedEventArgs e)
        {
            NetService.Instance.HandleMessage(sender, e.Packet.Message);
        }
    }
}
