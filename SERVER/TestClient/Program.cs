using Common.Network;
using Common.Proto;
using System.Diagnostics;
using System.Net.Sockets;

namespace TestClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
            var connection = new Connection(socket);
            connection.SendRequest(new UserLoginRequest() { Username = "Test", Password = "TestPwd" });
            await connection.StartAsync();
        }
    }
}