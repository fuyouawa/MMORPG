using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MMORPG.Common.Network
{
    public static class NetConfig
    {
        public static readonly int PacketHeaderSize = 8;
        public static readonly int MaxPacketSize = 1024 * 64;
        public static readonly int ServerPort = 11451;


        //public static IPAddress ServerIpAddress => IPAddress.Parse("117.72.68.24");
        public static IPAddress ServerIpAddress => IPAddress.Parse("127.0.0.1");
    }
}
