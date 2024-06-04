using Common.Network;
using GameServer.Model;
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
        public string ChannelName
        {
            get
            {
                var name = _remoteEpName;
                if (User != null)
                {
                    name += $"({User.UserId}:{User.Username}";
                    if (User.Player != null)
                    {
                        name += $":{User.Player.Name}";
                    }

                    name += ")";
                }

                return name;
            }
        }

        public User? User { get; private set; }
        public long LastActiveTime { get; set; }
        public LinkedListNode<NetChannel>? LinkedListNode { get; set; }

        private string _remoteEpName;

        public NetChannel(Socket socket) : base(socket)
        {
            _remoteEpName = _socket.RemoteEndPoint?.ToString() ?? "NULL";

            ConnectionClosed += OnConnectionClosed;
            ErrorOccur += OnErrorOccur;
            WarningOccur += OnWarningOccur;
        }

        public void SetUser(User user)
        {
            User = user;
        }

        private void OnWarningOccur(object? sender, WarningOccurEventArgs e)
        {
            Log.Warning($"[Channel:{ChannelName}] 出现警告:{e.Description}");
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
