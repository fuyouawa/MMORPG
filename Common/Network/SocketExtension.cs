using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network.Extension
{
    public static class SocketExtension
    {
        public static async Task<byte[]> ReadAsync(this Socket socket, int size)
        {
            Debug.Assert(size < NetConfig.MaxPacketSize && size >= 0);
            if (size == 0)
                return Array.Empty<byte>();

            var buffer = new byte[size];

            int readSizeTotal = 0;
            while (readSizeTotal < size)
            {
                var curReadSize = await socket.ReceiveAsync(
                    new ArraySegment<byte>(buffer, readSizeTotal, size - readSizeTotal),
                    SocketFlags.None);
                Debug.Assert(curReadSize > 0);
                readSizeTotal += curReadSize;
            }

            return buffer;
        }

        public static async Task<int> ReadInt32Async(this Socket socket)
        {
            var intBytes = await socket.ReadAsync(sizeof(int));
            return IPAddress.NetworkToHostOrder(BitConverter.ToInt32(intBytes));
        }
    }
}
