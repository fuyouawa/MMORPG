using Common.Network;
using Common.Proto;
using Common.Proto.Player;
using Common.Tool;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;
using System.Security.Cryptography;

namespace TestClient
{

    internal class Program
    {
        static async Task Main(string[] args)
        {
            //await TimeWheelTest.Start();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Async(a => a.Console())
                .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
            Log.Information("连接到服务器");
            var session = new NetSession(socket);
            //Task.Run(async () =>
            //{
            //    Console.WriteLine("开始压力测试");
            //    Console.ReadLine();
            //    for (int i = 0; i < 10000; i++)
            //    {
            //        session.SendAsync(new UserLoginRequest() { Username = $"Test{i}", Password = $"TestPwd{i}" }, null);
            //        await session.ReceiveAsync<UserLoginResponse>();
            //    }
            //});
            session.Send(new LoginRequest() { Username = $"Test", Password = $"TestPwd" }, null);
            //        await session.ReceiveAsync<UserLoginResponse>();
            session.PacketReceived += OnPacketReceived;
            session.SuddenPacketReceived += OnSuddenPacketReceived;
            await session.StartAsync();

            Console.ReadLine();
        }

        private static void OnSuddenPacketReceived(object? sender, SuddenPacketReceivedEventArgs e)
        {
            Console.WriteLine("突发消息接收!");
        }

        static private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            var session = sender as NetSession;
            Debug.Assert(session != null);
            UserService.Instance.HandleMessage(session, e.Packet.Message);
        }
    }

    class UserService : ServiceBase<UserService>
    {
        public void OnHandle(NetSession sender, LoginResponse response)
        {
        }
    }
}