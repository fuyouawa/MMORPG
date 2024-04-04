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
            connection.Send(new UserLoginRequest() { Username = "Test", Password = "TestPwd" });
            await connection.StartAsync();
        }
    }
}