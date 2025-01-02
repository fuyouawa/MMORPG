using MMORPG.Common.Proto;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MMORPG.Common.Network
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


    public class ErrorOccurEventArgs : EventArgs
    {
        public Exception Exception { get; }

        public ErrorOccurEventArgs(Exception ex)
        {
            Exception = ex;
        }
    }

    public class WarningOccurEventArgs : EventArgs
    {
        public string Description { get; }

        public WarningOccurEventArgs(string description)
        {
            Description = description;
        }
    }

    public class Connection
    {
        public static readonly int MaxSendQueueCount = 1024;

        public event EventHandler<ConnectionClosedEventArgs>? ConnectionClosed;
        public event EventHandler<PacketReceivedEventArgs>? PacketReceived;
        public event EventHandler<ErrorOccurEventArgs>? ErrorOccur;
        public event EventHandler<WarningOccurEventArgs>? WarningOccur;

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
            {
                // 如果为false, 说明是由报错引起的断开连接
                // 所以直接通知ConnectionClosed
                if (_closeConnectionByManual == false)
                {
                    ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(true));
                }
                else if (_closeConnectionByManual == true)
                {
                    WarningOccur?.Invoke(this, new WarningOccurEventArgs("尝试重复关闭同一个连接!"));
                }
                else
                {
                    WarningOccur?.Invoke(this, new WarningOccurEventArgs("尝试关闭一个已经断开连接的Socket!"));
                }
                return;
            }
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
                _closeConnectionByManual ??= true;      // 如果这个值被设置过, 说明可能是Error引起的关闭
                _socket.Close();
                ConnectionClosed?.Invoke(this, new ConnectionClosedEventArgs(true));
            }
        }

        public async Task SendAsync(Google.Protobuf.IMessage msg)
        {
            if (!_socket.Connected) {
                WarningOccur?.Invoke(this, new WarningOccurEventArgs("尝试向一个已经关闭的Socket发送数据!"));
                return;
            }
            try
            {
                var packet = new Packet(msg);
                var res = await _socket.SendAsync(packet.Pack(), SocketFlags.None);
                Debug.Assert(res > 0);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        public delegate void SuccessSentCallback(Connection sender, Packet packet);
        public void Send(Google.Protobuf.IMessage msg, SuccessSentCallback? successSentCallback = null)
        {
            if (!_socket.Connected)
            {
                WarningOccur?.Invoke(this, new WarningOccurEventArgs("尝试向一个已经关闭的Socket发送数据!"));
                return;
            }
            try
            {
                var packet = new Packet(msg);
                var buf = packet.Pack();
                _socket.BeginSend(buf, 0, buf.Length, SocketFlags.None, ar =>
                {
                    var res = _socket.EndSend(ar);
                    Debug.Assert(res > 0);
                    successSentCallback?.Invoke(this, packet);
                }, null);
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
                    Debug.Assert(size >= NetConfig.PacketHeaderSize && size < NetConfig.MaxPacketSize);
                    var msgId = await _socket.ReadInt32Async();
                    var buffer = await _socket.ReadAsync(size - NetConfig.PacketHeaderSize);
                    PacketReceived?.Invoke(this, new PacketReceivedEventArgs(new Packet(msgId, buffer)));
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        protected virtual void HandleError(Exception ex)
        {
            if (ex is SocketException ex1)
            {
                Debug.Assert(ex1.SocketErrorCode != SocketError.Success);

                if (_closeConnectionByManual == true) return;
                Debug.Assert(_closeConnectionByManual != false);
                _closeConnectionByManual = false;
                Close();
                return;
            }
            else if (ex is ObjectDisposedException ex2)
            {
                if (_closeConnectionByManual != null)
                {
                    WarningOccur?.Invoke(this, new WarningOccurEventArgs("在Socket被关闭后发生了ObjectDisposedException"));
                    return;
                }
            }
            ErrorOccur?.Invoke(this, new ErrorOccurEventArgs(ex));
        }
    }
}
