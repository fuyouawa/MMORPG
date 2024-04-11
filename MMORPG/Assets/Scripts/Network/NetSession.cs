using Common.Network;
using Common.Tool;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading.Tasks;

public class SuddenPacketReceivedEventArgs
{
    public Packet Packet { get; }

    public SuddenPacketReceivedEventArgs(Packet packet)
    {
        Packet = packet;
    }
}

public class NetSession : Connection
{
    /// <summary>
    /// 突发消息处理
    /// </summary>
    public event EventHandler<SuddenPacketReceivedEventArgs>? SuddenPacketReceived;

    public NetSession(Socket socket) : base(socket)
    {
        ConnectionClosed += OnConnectionClosed;
        ErrorOccur += OnErrorOccur;
        PacketReceived += OnPacketReceived;
    }

    //TODO 高水位处理
    private List<Packet> _receivedPackets = new List<Packet>();
    private TaskCompletionSource<bool> _receivedPacketTSC = new TaskCompletionSource<bool>();

    public async Task<T> ReceiveAsync<T>() where T : class, Google.Protobuf.IMessage
    {
        while (true)
        {
            await _receivedPacketTSC.Task;
            _receivedPacketTSC = new TaskCompletionSource<bool>();
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

    private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
    {
        Global.Logger.Debug($"[Channel] 接收来自服务器端的数据包:{e.Packet.Message.GetType()}");
        if (ProtoManager.Instance.IsEmergency(e.Packet.Message.GetType()))
        {
            SuddenPacketReceived?.Invoke(this, new SuddenPacketReceivedEventArgs(e.Packet));
            return;
        }
        lock (_receivedPackets)
        {
            _receivedPackets.Add(e.Packet);
        }
        _receivedPacketTSC.TrySetResult(true);
    }

    private void OnErrorOccur(object? sender, ErrorOccurEventArgs e)
    {
        Global.Logger.Error($"[Channel] 出现异常:{e.Exception}");
    }

    private void OnConnectionClosed(object? sender, ConnectionClosedEventArgs e)
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