using Common.Network;
using Common.Proto;
using GameServer.Db;
using GameServer.Network;
using GameServer.Service;
using GameServer.Tool;

namespace GameServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {

            //SqlDb.Connection.Insert(new DbPlayer()).ExecuteAffrows();
            GameServer server = new(NetConfig.ServerPort);
            await server.Run();
        }

    }
}