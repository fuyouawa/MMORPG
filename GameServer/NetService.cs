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
            new Connection(socket,
                new Connection.DataReceivedCallback(OnDataReceived),
                new Connection.DisconnectedCallback(OnDisconnected));

        }

        private static void OnDisconnected(Connection sender)
        {
            Console.WriteLine("客户端断开");
        }

        private static void OnDataReceived(Connection sender, byte[] data)
        {
            Proto.Package package = Proto.Package.Parser.ParseFrom(data);
            MessageRouter.Instance.AddMessage(sender, package);
        }
    }
}
