using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public class BytesPacket
    {
        public byte[] Data { get; init; }

        public BytesPacket(byte[] data)
        {
            Data = data;
        }

        public BytesPacket(Google.Protobuf.IMessage msg) : this(msg.ToByteArray()) { }

        public byte[] Pack()
        {
            var lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data.Length));
            var res = new byte[lengthBytes.Length + Data.Length];
            Array.Copy(lengthBytes, res, lengthBytes.Length);
            Array.Copy(Data, 0, res, lengthBytes.Length, Data.Length);
            return res;
        }
    }
}
