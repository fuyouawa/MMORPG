using Common.Network.Extension;
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
        public bool IsManual { get; init; }

        public ConnectionClosedEventArgs(bool isManual)
        {
            IsManual = isManual;
        }
    }

    public class PacketReceivedEventArgs : EventArgs
    {
        public BytesPacket Packet { get; init; }

        public PacketReceivedEventArgs(BytesPacket packet)
        {
            Packet = packet;
        }
    }

    public class SuccessSentEventArgs : EventArgs
    {
        public BytesPacket Packet { get; init; }

        public SuccessSentEventArgs(BytesPacket packet)
        {
            Packet = packet;
        }
    }

    public class ErrorOccurEventArgs : EventArgs
    {
        public Exception Exception { get; init; }

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
        protected Queue<BytesPacket> _pendingSendQueue = new();

        private bool? _closeConnectionByManual;


        public Connection(Socket socket)
        {
            _socket = socket;
        }

        public void Start()
        {
            Task.Run(ReceiveLoop);
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
                ErrorOccur?.Invoke(this, new(ex));
            }
            finally
            {
                _socket.Close();
                _closeConnectionByManual = true;
                ConnectionClosed?.Invoke(this, new(true));
            }
        }

        public void Send(Google.Protobuf.IMessage msg)
        {
            Send(new BytesPacket(msg));
        }


        public void Send(BytesPacket packet)
        {
            try
            {
                Debug.Assert(_socket.Connected);
                lock (_pendingSendQueue)
                {
                    var oldQueueCount = _pendingSendQueue.Count;
                    if (oldQueueCount > MaxSendQueueCount)
                    {
                        HighWaterMark?.Invoke(this, new());
                        return;
                    }
                    _pendingSendQueue.Enqueue(packet);
                    if (oldQueueCount > 0)
                        return;
                }
                SocketAsyncEventArgs asyncEventArgs = new();
                asyncEventArgs.SetBuffer(packet.Pack());
                asyncEventArgs.Completed += OnSent;
                _socket.SendAsync(asyncEventArgs);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void OnSent(object? sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success)
            {
                ErrorOccur?.Invoke(this, new(new SocketException((int)e.SocketError)));
                return;
            }
            else
            {
                SuccessSent?.Invoke(this, new(_pendingSendQueue.Peek()));
            }
            try
            {
                BytesPacket? pendingPacket = null;
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
                    SocketAsyncEventArgs asyncEventArgs = new();
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
                    var buffer = await _socket.ReadAsync(size);
                    PacketReceived?.Invoke(this, new(new(buffer)));
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
                        ConnectionClosed?.Invoke(this, new(false));
                        return;
                    default:
                        break;
                }
            }
            ErrorOccur?.Invoke(this, new(ex));
        }
    }
}
