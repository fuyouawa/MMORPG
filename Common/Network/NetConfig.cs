using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public static class NetConfig
    {
        public static readonly int MaxPacketSize = 1024 * 64;
        public static readonly int ServerPort = 11451;

        public static IPAddress ServerIpAddress => IPAddress.Parse("127.0.0.1");
    }
}
