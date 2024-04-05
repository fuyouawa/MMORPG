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

        private bool? _closeConnectionByManual;


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

        public async Task SendAsync(Google.Protobuf.IMessage msg)
        {
            try
            {
                var packet = new Packet(msg);
                var res = await _socket.SendAsync(packet.Pack(), SocketFlags.None);
                Debug.Assert(res > 0);
                SuccessSent?.Invoke(this, new SuccessSentEventArgs(packet));
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
                    PacketReceived?.Invoke(this, new PacketReceivedEventArgs(new Packet(msgID, buffer)));
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
