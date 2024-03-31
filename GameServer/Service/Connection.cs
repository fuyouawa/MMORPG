using Common.Network;
using Common.Network.Extension;
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
    public class SocketAsyncException : Exception
    {
        private SocketError _error;

        public SocketError Error
        {
            get { return _error; }
            set { _error = value; }
        }


        public SocketAsyncException(SocketError error)
        {
            _error = error;
        }
    }

    public class Connection
    {
        public static readonly int MaxSendQueueCount = 1024;

        public delegate void EventHandler<TEventArgs>(Connection sender, TEventArgs e);
        public delegate void EventHandler(Connection sender);

        public event EventHandler<ObjectDisposedException?>? SessionClosed;
        public event EventHandler<Packet>? PacketReceived;
        public event EventHandler<Exception>? ErrorOccur;
        public event EventHandler? HighWaterMark;

        private Socket _socket;
        private Queue<Packet> _sendQueue = new();
        private bool? _closeConnectionByServer;

        //TODO 可读性更高的SessionName
        private string SessionName => _socket.RemoteEndPoint.ToString();


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
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (ObjectDisposedException)
            {
                Global.Logger.Warn($"尝试关闭已经关闭的socket!");
                return;
            }
            catch (Exception ex)
            {
                ErrorOccur?.Invoke(this, ex);
            }
            finally
            {
                _socket.Close();
                _closeConnectionByServer = true;
                Global.Logger.Info($"关闭对({SessionName})的链接!");
            }
        }


        public void Send(Packet packet)
        {
            try
            {
                Debug.Assert(_socket.Connected);
                lock (_sendQueue)
                {
                    var oldQueueCount = _sendQueue.Count;
                    if (oldQueueCount > MaxSendQueueCount)
                    {
                        Global.Logger.Error($"({SessionName})的发送队列超出最高水位!");
                        HighWaterMark?.Invoke(this);
                        return;
                    }
                    _sendQueue.Enqueue(packet);
                    if (oldQueueCount > 0)
                        return;
                }
                SocketAsyncEventArgs asyncEventArgs = new();
                asyncEventArgs.SetBuffer(packet.Pack());
                asyncEventArgs.Completed += HandleSent;
                _socket.SendAsync(asyncEventArgs);
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void HandleSent(object? sender, SocketAsyncEventArgs e)
        {
            if (e.SocketError != SocketError.Success) {
                Global.Logger.Error($"发送数据时出现异常:{e.SocketError}");
                Close();
                ErrorOccur?.Invoke(this, new SocketAsyncException(e.SocketError));
            }
            try
            {
                Packet? pendingPacket = null;
                lock (_sendQueue)
                {
                    _sendQueue.Dequeue();
                    if (_sendQueue.Count > 0) {
                        pendingPacket = _sendQueue.Peek();
                    }
                }
                if (pendingPacket != null)
                {
                    SocketAsyncEventArgs asyncEventArgs = new();
                    asyncEventArgs.SetBuffer(pendingPacket.Pack());
                    asyncEventArgs.Completed += HandleSent;
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
                    PacketReceived?.Invoke(this, new(buffer));
                }
            }
            catch (Exception ex)
            {
                HandleError(ex);
            }
        }

        private void HandleError(Exception ex)
        {
            if (ex is ObjectDisposedException e)
            {
                Global.Logger.Error($"({SessionName})对端关闭链接:{ex}");
                _closeConnectionByServer = false;
                SessionClosed?.Invoke(this, e);
            }
            else
            {
                Global.Logger.Error($"{SessionName}出现异常:{ex}");
                Close();
                ErrorOccur?.Invoke(this, ex);
            }
        }
    }
}
