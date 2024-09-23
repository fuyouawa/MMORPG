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
        /// <summary>
        /// 数据包头部大小
        /// </summary>
        public static readonly int PacketHeaderSize = 8;
        /// <summary>
        /// 数据包最大大小
        /// </summary>
        public static readonly int MaxPacketSize = 1024 * 64;
        /// <summary>
        /// 服务器端口
        /// </summary>
        public static readonly int ServerPort = 11451;


        /// <summary>
        /// 服务器Ip地址
        /// </summary>
        public static readonly IPAddress ServerIpAddress = IPAddress.Parse("127.0.0.1");
    }
}
