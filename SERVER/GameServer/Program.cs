
using Common.Network;
using Common.Proto;
using GameServer.Service;
using GameServer.Tool;

namespace GameServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TcpServer server = new(NetConfig.ServerPort);
            await server.Run();
        }
    }
}