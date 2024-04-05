using Common.Network;
using GameServer.Network;
using GameServer.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer
{
    public class GameServer : TcpServer
    {
        public GameServer(int port) : base(port) { }

        protected override void OnPacketReceived(object? sender, PacketReceivedEventArgs e)
        {
            UserService.Instance.HandleMessage(sender, e.Packet.Message);
            PlayerService.Instance.HandleMessage(sender, e.Packet.Message);
        }
    }
}
