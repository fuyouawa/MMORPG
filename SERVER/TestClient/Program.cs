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
            var connection = new Connection(socket);
            for (int i = 0; i < 100; i++)
            {
                Console.WriteLine($"发送第{i}个数据包");
                await connection.SendAsync(new UserLoginRequest() { Username = $"Test_{i}", Password = $"TestPwd_{i}" });
            }
            await connection.StartAsync();
        }
    }
}