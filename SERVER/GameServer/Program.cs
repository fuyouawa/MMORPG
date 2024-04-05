
using Common.Network;
using Common.Proto;
using GameServer.Network;
using GameServer.Service;
using GameServer.Tool;

namespace GameServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            GameServer server = new(NetConfig.ServerPort);
            await server.Run();
        }

    }
}