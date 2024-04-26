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
            //    UserId = 1,
            //    MapId = 1,
            //    X = 0,
            //    Y = 2,
            //    Z = 0,
            //};
            //SqlDb.Connection.Insert(character).ExecuteAffrows();
            GameServer server = new(NetConfig.ServerPort);
            await server.Run();
        }

    }
}