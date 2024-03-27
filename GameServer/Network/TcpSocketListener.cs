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
    /// 负责监听TCP网络端口，异步接收Socket连接
    /// </summary>
    public class TcpSocketListener
    {
        private IPEndPoint endPoint;
        private Socket serverSocket;    //服务端监听对象

        public event EventHandler<Socket> SocketConnected; //客户端接入事件

        public TcpSocketListener(string host, int port)
        {
            endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        }

        public void Start()
        {
            if (!IsRunning)
            {
                serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                serverSocket.Bind(endPoint);
                serverSocket.Listen();
                Console.WriteLine("开始监听端口：" + endPoint.Port);

                SocketAsyncEventArgs args = new SocketAsyncEventArgs();
                args.Completed += OnAccept; //当有人连入的时候
                serverSocket.AcceptAsync(args);
                
            }
        }

        private void OnAccept(object? sender, SocketAsyncEventArgs e)
        {
            //真的有人连进来
            if (e.SocketError == SocketError.Success)
            {
                Socket client = e.AcceptSocket; //连入的人
                if (client != null)
                {
                    SocketConnected?.Invoke(this, client);
                }
                
            }

            //继续接收下一位
            e.AcceptSocket = null;
            serverSocket.AcceptAsync(e);
        }

        public bool IsRunning
        {
            get { return serverSocket != null; }
        }

        public void Stop()
        {
            if (serverSocket == null)
                return;
            serverSocket.Close();
            serverSocket = null;
        }

    }
}
