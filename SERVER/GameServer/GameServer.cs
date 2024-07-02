using MMORPG.Common.Network;
using GameServer.Network;
using GameServer.NetService;
using Service;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;
using MMORPG.Common.Tool;
using GameServer.Manager;
using Serilog;

namespace GameServer
{
    public class GameServer
    {
        private Socket _serverSocket;
        private LinkedList<NetChannel> _channels;
        private TimeWheel _connectionCleanupTimer;

        public GameServer(int port)
        {
            _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            _serverSocket.Bind(new IPEndPoint(IPAddress.Parse("0.0.0.0"), port));
            _channels = new();
            _connectionCleanupTimer = new(1000);
        }

        public async Task Run()
        {
            Log.Information("[Server] 开启服务器");

            Log.Information("[Server] 开始初始化Manager...");
            UpdateManager.Instance.Start();
            Log.Information("[Server] Manager初始化完成");

            _serverSocket.Listen();
            //_connectionCleanupTimer.Start();
            while (true)
            {
                var socket = await _serverSocket.AcceptAsync();
                Log.Information($"[Server] 客户端连接:{socket.RemoteEndPoint}");
                NetChannel channel = new(socket);
                OnNewChannelConnection(channel);
                Task.Run(channel.StartAsync);
            }
        }

        private void OnNewChannelConnection(NetChannel sender)
        {
            lock (_channels)
            {
                var node = _channels.AddLast(sender);
                sender.LinkedListNode = node;
            }

            sender.PacketReceived += OnPacketReceived;
            sender.ConnectionClosed += OnConnectionClosed;
            sender.LastActiveTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            _connectionCleanupTimer.AddTask(ChannelConfig.CleanupMs, (task) =>
            {
                var now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond; ;
                var duration = now - sender.LastActiveTime;
                if (duration > ChannelConfig.CleanupMs)
                {
                    // sender已关闭也不会产生错误
                    sender.Close();
                }
                else
                {
                    _connectionCleanupTimer.AddTask(ChannelConfig.CleanupMs, task.Action);
                }
            });

            UserService.Instance.OnConnect(sender);
            CharacterService.Instance.OnConnect(sender);
            MapService.Instance.OnConnect(sender);
            PlayerService.Instance.OnConnect(sender);
            NpcService.Instance.OnConnect(sender);
        }

        private void OnConnectionClosed(object? sender, ConnectionClosedEventArgs e)
        {
            var channel = sender as NetChannel;
            Debug.Assert(channel != null);
            if (channel == null) return;
            
            UserService.Instance.OnChannelClosed(channel);
            CharacterService.Instance.OnChannelClosed(channel);
            MapService.Instance.OnChannelClosed(channel);
            PlayerService.Instance.OnChannelClosed(channel);
            NpcService.Instance.OnChannelClosed(channel);
            lock (_channels)
            {
                if (channel.LinkedListNode != null)
                {
                    _channels.Remove(channel.LinkedListNode);
                }
            }
        }

        private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            var channel = sender as NetChannel;
            Debug.Assert(channel != null);

            channel.LastActiveTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;

            UserService.Instance.HandleMessage(channel, e.Packet.Message);
            CharacterService.Instance.HandleMessage(channel, e.Packet.Message);
            MapService.Instance.HandleMessage(channel, e.Packet.Message);
            PlayerService.Instance.HandleMessage(channel, e.Packet.Message);
            FightService.Instance.HandleMessage(channel, e.Packet.Message);
            InventoryService.Instance.HandleMessage(channel, e.Packet.Message);
            NpcService.Instance.HandleMessage(channel, e.Packet.Message);
        }
    }
}
