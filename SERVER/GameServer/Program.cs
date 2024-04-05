
using Common.Network;
using Common.Proto;
using GameServer.Network;
using GameServer.Service;
using GameServer.Tool;

namespace GameServer
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            TcpServer server = new(NetConfig.ServerPort);
            server.PacketReceived += OnPacketReceived;
            await server.Run();
        }

        static private void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            UserService.Instance.HandleMessage(sender, e.Packet.Message);
            PlayerService.Instance.HandleMessage(sender, e.Packet.Message);
        }
    }
}