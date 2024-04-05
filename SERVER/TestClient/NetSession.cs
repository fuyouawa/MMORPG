using Common.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    public class NetSession : Connection
    {
        public NetSession(Socket socket) : base(socket)
        {
            ConnectionClosed += OnConnectionClosed;
            ErrorOccur += OnErrorOccur;
            PacketReceived += OnPacketReceived;
        }

        //TODO 高水位处理
        private List<Packet> _receivedPackets = new List<Packet>();
        private TaskCompletionSource _receivedPacketTSC = new TaskCompletionSource();

        public async Task<T> ReceiveAsync<T>() where T : class, Google.Protobuf.IMessage
        {
            while (true)
            {
                await _receivedPacketTSC.Task;
                _receivedPacketTSC = new TaskCompletionSource();
                Packet? packet;
                lock (_receivedPackets)
                {
                    packet = _receivedPackets.Find(packet => { return packet.Message.GetType() == typeof(T); });
                    if (packet != null)
                        _receivedPackets.Remove(packet);
                    else
                        continue;
                }
                var res = packet.Message as T;
                Debug.Assert(res != null);
                return res;
            }
        }

        private void OnPacketReceived(Connection sender, PacketReceivedEventArgs e)
        {
            Global.Logger.Info($"[Channel] 接收来自服务器端的数据包:{e.Packet.Message.GetType()}");
            lock (_receivedPackets)
            {
                _receivedPackets.Add(e.Packet);
            }
            _receivedPacketTSC.TrySetResult();
        }

        private void OnErrorOccur(Connection sender, ErrorOccurEventArgs e)
        {
            Global.Logger.Error($"[Channel] 出现异常:{e.Exception}");
        }

        private void OnConnectionClosed(Connection sender, ConnectionClosedEventArgs e)
        {
            if (e.IsManual)
            {
                Global.Logger.Info($"[Channel] 关闭对服务器端的链接!");
            }
            else
            {
                Global.Logger.Info($"[Channel] 对端关闭链接");
            }
        }
    }
}
