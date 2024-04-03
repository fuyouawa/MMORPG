using Common.Network;
using Common.Proto;
using System.Net.Sockets;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(NetConfig.ServerIpAddress, NetConfig.ServerPort);
            Console.WriteLine("链接到服务器");
            Connection connection = new(socket);
            connection.Start();
            var msg = new NetMessage()
            {
                Request = new()
                {
                    UserLogin = new()
                    {
                        Username = "Test",
                        Password = "TestPwd",
                    }
                }
            };
            Console.WriteLine("发送登录请求");
            connection.Send(msg);
            Console.ReadLine();
        }
    }
}