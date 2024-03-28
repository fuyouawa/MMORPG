using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    /// <summary>
    /// 客户端网络连接
    /// 职责：发送消息，关闭连接，断开回调，接收消息回调
    /// </summary>
    public class Connection
    {
        public delegate void DataReceivedCallback(Connection sender, byte[] data);
        public delegate void DisconnectedCallback(Connection sender);

        public Socket socket;
        private DataReceivedCallback dataReceivedCallback;
        private DisconnectedCallback disconnectedCallback;
        
        public Connection(Socket socket, DataReceivedCallback cb1, DisconnectedCallback cb2)
        {
            this.socket = socket;
            this.dataReceivedCallback = cb1;
            this.disconnectedCallback = cb2;

            var lfd = new LengthFieldDecoder(socket, 64 * 1024, 0, 4, 0, 4);
            lfd.DataReceived += OnDataReceived;
            lfd.Disconnected += OnDisconnected;
            lfd.Start();
        }

        private void OnDisconnected(Socket socket)
        {
            disconnectedCallback?.Invoke(this);
        }

        private void OnDataReceived(byte[] data)
        {
            dataReceivedCallback(this, data);
        }

        /// <summary>
        /// 主动关闭连接
        /// </summary>
        public void Close()
        {
            try { socket.Shutdown(SocketShutdown.Both); } catch { }

            socket.Close();
            socket = null;
            disconnectedCallback(this);
        }

        private Proto.Package _package = null;

        public Proto.Request Request
        {
            get
            {
                if (_package == null)
                {
                    _package = new();
                }
                if (_package.Request == null) { 
                    _package.Request = new();
                }
                return _package.Request;
            }
        }

        public Proto.Response Response
        {
            get
            {
                if (_package == null)
                {
                    _package = new();
                }
                if (_package.Response == null)
                {
                    _package.Response = new();
                }
                return _package.Response;
            }
        }


        public void Send()
        {
            if (_package != null) Send(_package);
            _package = null;
        }

        public void Send(Proto.Package package)
        {
            byte[] data = null;
            using(MemoryStream ms = new MemoryStream())
            {
                package.WriteTo(ms);
                data = new byte[4 + ms.Length];
                Buffer.BlockCopy(BitConverter.GetBytes(ms.Length), 0, data, 0, 4);
                Buffer.BlockCopy(ms.GetBuffer(), 0, data, 4, (int)ms.Length);
            }
            Send(data,0,data.Length);
        }

        public void Send(byte[] data, int offset, int count)
        {
            lock(this)
            {
                if (socket.Connected)
                {
                    socket.BeginSend(data, offset, count, SocketFlags.None, new AsyncCallback(SendCallback), socket);
                }
            }
        }

        private void SendCallback(IAsyncResult ar)
        {
            int len = socket.EndSend(ar);
        }
    }
}
