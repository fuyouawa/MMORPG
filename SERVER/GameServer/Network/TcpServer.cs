using Common.Network;
using GameServer.Tool;
using System.Net;
using System.Net.Sockets;

namespace GameServer.Network
{
    public class TcpServer
    {
        private Socket _serverSocket;
        private List<Channel> _channels = new();

        public event Channel.EventHandler<PacketReceivedEventArgs> PacketReceived;

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
                Channel channel = new(socket);
                channel.PacketReceived = PacketReceived;
                Task.Run(channel.StartAsync);
                _channels.Add(channel);
            }
        }
    }
}
