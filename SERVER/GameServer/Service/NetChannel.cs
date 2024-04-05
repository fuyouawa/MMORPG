using Common.Network;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Service
{
    public class NetChannel : Connection
    {
        //TODO 可读性更高的ChannelName
        public string ChannelName => _socket.RemoteEndPoint.ToString();

        public NetChannel(Socket socket) : base(socket)
        {
            ConnectionClosed += OnConnectionClosed;
            ErrorOccur += OnErrorOccur;
            PacketReceived += OnPacketReceived;
        }

        private void OnPacketReceived(Connection sender, PacketReceivedEventArgs e)
        {
            Global.Logger.Info($"[Channel] 接收来自{ChannelName}的数据包:{e.Packet.Message.GetType()}");
        }


        private void OnErrorOccur(Connection sender, ErrorOccurEventArgs e)
        {
            Global.Logger.Error($"[Channel] {ChannelName}出现异常:{e.Exception}");
        }

        private void OnConnectionClosed(Connection sender, ConnectionClosedEventArgs e)
        {
            if (e.IsManual)
            {
                Global.Logger.Info($"[Channel] 成功关闭对{ChannelName}的链接!");
            }
            else
            {
                Global.Logger.Info($"[Channel] {ChannelName}对端关闭链接");
            }
        }
    }
}
