using Common.Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public class ConnectionClosedEventArgs : EventArgs
    {
        public bool IsManual { get; }

        public ConnectionClosedEventArgs(bool isManual)
        {
            IsManual = isManual;
        }
    }

    public class PacketReceivedEventArgs : EventArgs
    {
        public Packet Packet { get; }

        public PacketReceivedEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }

    public class SuccessSentEventArgs : EventArgs
    {
        public Packet Packet { get; }

        public SuccessSentEventArgs(Packet packet)
        {
            Packet = packet;
        }
    }

    public class ErrorOccurEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ErrorOccurEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }

    public class HighWaterMarkEventArgs : EventArgs
    {
        public HighWaterMarkEventArgs()
        {
        }
    }


    public class Connection
    {
        public static readonly int MaxSendQueueCount = 1024;

        public event EventHandler<ConnectionClosedEventArgs>? ConnectionClosed;
        public event EventHandler<PacketReceivedEventArgs>? PacketReceived;
        public event EventHandler<SuccessSentEventArgs>? SuccessSent;
        public event EventHandler<ErrorOccurEventArgs>? ErrorOccur;
        public event EventHandler<HighWaterMarkEventArgs>? HighWaterMark;

        protected Socket _socket;
        protected Queue<Packet> _pendingSendQueue = new Queue<Packet>();

        private bool? _closeConnectionByManual;
        private TaskCompletionSource<Packet> _newReceivedPacketTSC = new TaskCompletionSource<Packet>();


        public Connection(Socket socket)
        {
            _socket = socket;
        }

        public async Task StartAsync()
        {
            await ReceiveLoop();
        }


        public void Close()
        {
            if (!_socket.Connected)
                return;
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (Exception ex)
            {
                ErrorOccur?.Invoke(this, new ErrorOccurEventArgs(ex));
            }
            finally
            {
                _socket.Close();
                _closeConnectionByManual = true;
                ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(true));
            }
        }

        public void Send(Google.Protobuf.IMessage msg)
        {
            try
            {
                var packet = new Packet(msg);
                Debug.Assert(_socket.Connected);
                lock (_pendingSendQueue)
                {
                    var oldQueueCount = _pendingSendQueue.Count;
                    if (oldQueueCount > MaxSendQueueCount)
                    {
                        HighWaterMark?.Invoke(this, new HighWaterMarkEventArgs());
                        return;
                    }
                    _pendingSendQueue.Enqueue(packet);
                    if (oldQueueCount > 0)
                        return;
                }
                SocketAsyncEventArgs asyncEventArgs = new SocketAsyncEventArgs();
                asyncEventArgs.SetBuffer(packet.Pack());
                asyncEventArgs.Completed += OnSent;
                _socket.SendAsync(asyncEventArgs);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public async Task<T> ReceiveAsync<T>() where T : class, Google.Protobuf.IMessage
        {
            while (true)
            {
                var packet = await _newReceivedPacketTSC.Task;
                Debug.Assert(packet != null);
                if (packet.Message.GetType() == typeof(T))
                {
                    var res = packet.Message as T;
                    Debug.Assert(res != null);
                    return res;
                }
            }
        }

        private void OnSent(object? sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                ErrorOccur?.Invoke(this, new ErrorOccurEventArgs(new SocketException((int)e.SocketError)));
                return;
            }
            else
            {
                SuccessSent?.Invoke(this, new SuccessSentEventArgs(_pendingSendQueue.Peek()));
            }
            try
            {
                Packet? pendingPacket = null;
                lock (_pendingSendQueue)
                {
                    _pendingSendQueue.Dequeue();
                    if (_pendingSendQueue.Count > 0)
                    {
                        pendingPacket = _pendingSendQueue.Peek();
                    }
                }
                if (pendingPacket != null)
                {
                    var asyncEventArgs = new SocketAsyncEventArgs();
                    asyncEventArgs.SetBuffer(pendingPacket.Pack());
                    asyncEventArgs.Completed += OnSent;
                    _socket.SendAsync(asyncEventArgs);
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private async Task ReceiveLoop()
        {
            try
            {
                while (_socket.Connected)
                {
                    var size = await _socket.ReadInt32Async();
                    Debug.Assert(size > 0 && size < NetConfig.MaxPacketSize);
                    var msgID = await _socket.ReadInt32Async();
                    var buffer = await _socket.ReadAsync(size);
                    var packet  = new Packet(msgID, buffer);
                    _newReceivedPacketTSC.SetResult(packet);
                    PacketReceived?.Invoke(this, new PacketReceivedEventArgs(packet));
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void HandleError(Exception ex)
        {
            if (ex is SocketException socketEx)
            {
                Debug.Assert(socketEx.SocketErrorCode != SocketError.Success);
                switch (socketEx.SocketErrorCode)
                {
                    case SocketError.ConnectionReset:
                        if (_closeConnectionByManual == true) return;
                        _closeConnectionByManual = false;
                        ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(false));
                        return;
                    default:
                        break;
                }
            }
            ErrorOccur?.Invoke(this, new ErrorOccurEventArgs(ex));
        }
    }
}
