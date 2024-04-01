using Common.Network;
using System.Net.Sockets;

namespace TestClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Socket socket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect("127.0.0.1", NetConfig.ServerPort);
            Console.WriteLine("链接到服务器");
            Connection connection = new(socket);
            Console.ReadLine();
        }
    }
}