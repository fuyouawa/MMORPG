using Common.Network;
using System.Net.Sockets;

namespace MMORPG.Tool
{
    //public class EmergencyPacketReceivedEventArgs
    //{
    //    public Packet Packet { get; }

    //    public EmergencyPacketReceivedEventArgs(Packet packet)
    //    {
    //        Packet = packet;
    //    }
    //}
    public class NetSession : Connection
    {
        public NetSession(Socket socket) : base(socket)
        {
            ConnectionClosed += OnConnectionClosed;
            ErrorOccur += OnErrorOccur;
            WarningOccur += OnWarningOccur;
            //PacketReceived += OnPacketReceived;
        }

        ////TODO 高水位处理
        //private List<Packet> _receivedPackets = new List<Packet>();
        //private TaskCompletionSource<bool> _receivedPacketTSC = new TaskCompletionSource<bool>();

        //public async Task<T> ReceiveAsync<T>() where T : class, Google.Protobuf.IMessage
        //{
        //    while (true)
        //    {
        //        //await _receivedPacketTSC.Task;
        //        //_receivedPacketTSC = new TaskCompletionSource<bool>();

        //        await Task.Delay(100);
        //        Packet? packet;
        //        lock (_receivedPackets)
        //        {
        //            packet = _receivedPackets.Find(packet => { return packet.Message.GetType() == typeof(T); });
        //            if (packet != null)
        //            {
        //                //UnityEngine.Debug.Log(typeof(T));
        //                _receivedPackets.Remove(packet);
        //            }
        //            else
        //            {
        //                continue;
        //            }
        //        }
        //        var res = packet.Message as T;
        //        Debug.Assert(res != null);
        //        return res;
        //    }
        //}

        //private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        //{
        //    Log.Information($"[Channel] 接收数据包:{e.Packet.Message.GetType()}");
        //    if (ProtoManager.Instance.IsEmergency(e.Packet.Message.GetType()))
        //    {
        //        EmergencyPacketReceived?.Invoke(this, new EmergencyPacketReceivedEventArgs(e.Packet));
        //        return;
        //    }
        //    lock (_receivedPackets)
        //    {
        //        _receivedPackets.Add(e.Packet);
        //    }

        //    //_receivedPacketTSC.TrySetResult(true);
        //}

        private void OnErrorOccur(object sender, ErrorOccurEventArgs e)
        {
            Log.Error("Channel", $"出现异常:{e.Exception}");
        }

        private void OnWarningOccur(object sender, WarningOccurEventArgs e)
        {
            Log.Warn("Channel", $"出现警告:{e.Description}");
        }

        private void OnConnectionClosed(object sender, ConnectionClosedEventArgs e)
        {
            if (e.IsManual)
            {
                Log.Info("Channel", $"关闭对服务器端的链接!");
            }
            else
            {
                Log.Info("Channel", $"对端关闭链接");
            }
        }
    }
}
