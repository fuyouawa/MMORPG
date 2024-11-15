﻿using MMORPG.Common.Network;
using MMORPG.Common.Proto;
using GameServer.Db;
using GameServer.Manager;
using GameServer.Network;
using GameServer.NetService;
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
                .MinimumLevel.Debug()
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();


            //var character = new DbCharacter("jj", 1, 1, 1, 1, 1, 1, 1, 1);
            //SqlDb.Connection.Insert(character).ExecuteAffrows();
            GameServer server = new(NetConfig.ServerPort);
            await server.Run();
        }

    }
}