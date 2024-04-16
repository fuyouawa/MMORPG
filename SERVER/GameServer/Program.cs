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


            //var character = new DbCharacter()
            //{
            //    Exp = 0,
            //    Gold = 0,
            //    Hp = 0,
            //    Id = 1,
            //    JobId = 0,
            //    Level = 0,
            //    Mp = 0,
            //    Name = "sb",
            //    PlayerId = 1,
            //    SpaceId = 1,
            //    X = 73,
            //    Y = 22,
            //    Z = 43,
            //};
            //SqlDb.Connection.Insert(character).ExecuteAffrows();
            GameServer server = new(NetConfig.ServerPort);
            await server.Run();
        }

    }
}