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
    public class NetConnection
    {
        public delegate void DataReceivedCallback(NetConnection sender, byte[] data);
        public delegate void DisconnectedCallback(NetConnection sender);

        public Socket socket;
        private DataReceivedCallback dataReceivedCallback;
        private DisconnectedCallback disconnectedCallback;
        
        public NetConnection(Socket socket, DataReceivedCallback cb1, DisconnectedCallback cb2)
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

    }
}
