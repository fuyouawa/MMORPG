using Common.Network;
using Common.Proto;
using GameServer.Db;
using GameServer.Network;
using GameServer.Service;
using GameServer.Tool;
using Serilog;
using Serilog.Events;

namespace GameServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();
            //SqlDb.Connection.Insert(new DbPlayer()).ExecuteAffrows();
            GameServer server = new(NetConfig.ServerPort);
            await server.Run();
        }

    }
}