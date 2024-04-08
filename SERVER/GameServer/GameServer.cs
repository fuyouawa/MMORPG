using Common.Network;
using GameServer.Network;
using GameServer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using GameServer.Tool;
using GameServer.Model;
using System.Threading.Channels;
using System.Diagnostics;

namespace GameServer
{
    public class GameServer
    {
        private Socket _serverSocket;
        private LinkedList<NetChannel> _channels = new();
        private Timer _connectionCleanupTimer;

        public GameServer(int port)
        {
            _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
        }

        public async Task Run()
        {
            Global.Logger.Info("[Server] 开启服务器");
            _serverSocket.Listen();
            _connectionCleanupTimer = new Timer(ConnectionCleanup, null, TimeSpan.Zero, TimeSpan.FromSeconds(5));
            while (true)
            {
                var socket = await _serverSocket.AcceptAsync();
                Global.Logger.Info($"[Server] 客户端连接:{socket.RemoteEndPoint}");
                NetChannel channel = new(socket);
                OnNewChannelConnection(channel);
                Task.Run(channel.StartAsync);
            }
        }

        private void ConnectionCleanup(object? state)
        {
            var now = DateTime.Now;
            lock (_channels)
            {
                for (var node = _channels.First; node != null;)
                {
                    var next = node.Next;
                    var duration = now - node.Value.LastActiveTime;
                    if (duration.TotalSeconds > 10)
                    {
                        node.Value.Close();
                    }
                    node = next;
                }
            }
        }

        private void OnNewChannelConnection(NetChannel sender)
        {
            sender.PacketReceived += OnPacketReceived;
            sender.ConnectionClosed += OnConnectionClosed;
            sender.LastActiveTime = DateTime.Now;

            PlayerService.Instance.OnConnect(sender);
            SpaceService.Instance.OnConnect(sender);
        }

        private void OnConnectionClosed(object? sender, ConnectionClosedEventArgs e)
        {
            var channel = sender as NetChannel;
            Debug.Assert(channel != null);

            PlayerService.Instance.OnChannelClosed(channel);
            SpaceService.Instance.OnChannelClosed(channel);

            lock (_channels)
            {
                _channels.Remove(channel);
            }
        }

        private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            var channel = sender as NetChannel;
            Debug.Assert(channel != null);

            channel.LastActiveTime = DateTime.Now;

            PlayerService.Instance.HandleMessage(channel, e.Packet.Message);
            SpaceService.Instance.HandleMessage(channel, e.Packet.Message);

            lock (_channels)
            {
                _channels.AddLast(channel);
            }
        }
    }
}
