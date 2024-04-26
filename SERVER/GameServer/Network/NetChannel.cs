using Common.Network;
using GameServer.Unit;
using GameServer.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    public class NetChannel : Connection
    {
        //TODO 可读性更高的ChannelName
        public string ChannelName { get; }

        public User? User { get; set; }
        public long LastActiveTime { get; set; }
        public LinkedListNode<NetChannel>? LinkedListNode { get; set; }

        public NetChannel(Socket socket) : base(socket)
        {
            var name = _socket.RemoteEndPoint?.ToString();
            ChannelName = name ?? "Unknown";

            ConnectionClosed += OnConnectionClosed;
            ErrorOccur += OnErrorOccur;
            WarningOccur += OnWarningOccur;
            PacketReceived += OnPacketReceived;
        }

        private void OnWarningOccur(object? sender, WarningOccurEventArgs e)
        {
            Log.Warning($"[Channel:{ChannelName}] 出现警告:{e.Description}");
        }

        private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            Log.Debug($"[Channel:{ChannelName}] 接收到数据包:{e.Packet.Message.GetType()}");
        }

        private void OnErrorOccur(object? sender, ErrorOccurEventArgs e)
        {
            Log.Error($"[Channel:{ChannelName}] 出现异常:{e.Exception}");
        }

        private void OnConnectionClosed(object? sender, ConnectionClosedEventArgs e)
        {
            if (e.IsManual)
            {
                Log.Information($"[Channel:{ChannelName}] 由服务器关闭链接");
            }
            else
            {
                Log.Information($"[Channel:{ChannelName}] 对端关闭链接");
            }
        }
    }
}
