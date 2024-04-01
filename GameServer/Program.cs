
using Common.Network;
using GameServer.Service;

namespace GameServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            TcpServer server = new(NetConfig.ServerPort);
            server.Start();
            Console.ReadLine();
        }
    }
}