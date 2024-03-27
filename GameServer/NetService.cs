using Common.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Network
{
    /// <summary>
    /// 网络服务
    /// </summary>
    public class NetService
    {
        TcpSocketListener listener = null;
        public void Init(int port)
        {
            listener = new TcpSocketListener("0.0.0.0", port);
            listener.SocketConnected += OnClientConnected;
        }

        public void Start()
        {
            listener.Start();
        }
        private static void OnClientConnected(object? sender, Socket socket)
        {
            var ipe = socket.RemoteEndPoint as IPEndPoint;
            Console.WriteLine("客户端接入：" + ipe.Address);
            new NetConnection(socket,
                new NetConnection.DataReceivedCallback(OnDataReceived),
                new NetConnection.DisconnectedCallback(OnDisconnected));

        }

        private static void OnDisconnected(NetConnection sender)
        {
            Console.WriteLine("客户端断开");
        }

        private static void OnDataReceived(NetConnection sender, byte[] data)
        {
            //MessageRouter.Instance.AddMessage(sender, );
        }
    }
}
