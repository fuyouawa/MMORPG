using Common.Network;
using Common.Proto;
using Common.Proto.User;
using Common.Tool;
using System.Diagnostics;
using System.Net.Sockets;
using System.Reflection;

namespace TestClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
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
            session.SendAsync(new UserLoginRequest() { Username = $"Test", Password = $"TestPwd" }, null);
            //        await session.ReceiveAsync<UserLoginResponse>();
            session.PacketReceived += OnPacketReceived;
            await session.StartAsync();
        }

        static private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            UserService.Instance.HandleMessage(sender, e.Packet.Message);
        }
    }

    class UserService : ServiceBase<UserService>
    {
        public void OnHandle(object? sender, UserLoginResponse response)
        {
            Console.WriteLine($"服务器响应登录：{response.Message}");
        }
    }
}