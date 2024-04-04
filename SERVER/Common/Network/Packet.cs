using Common.Tool;
using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public class Packet
    {
        private Type? _messageType;
        public Type MessageType => _messageType ??= ProtoManager.Instance.IDToType(MessageID);

        public int MessageID { get; }
        public byte[] Data { get; }

        public Packet(IMessage msg)
        {
            MessageID = ProtoManager.Instance.TypeToID(msg.GetType());
            Data = msg.ToByteArray();
        }
        public Packet(int msgID, byte[] buffer)
        {
            MessageID = msgID;
            Data = buffer;
        }

        public byte[] Pack()
        {
            var lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data.Length));
            var idBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(MessageID));
            var res = new byte[8 + Data.Length];
            Array.Copy(lengthBytes, res, 4);
            Array.Copy(idBytes, 0, res, 4, 4);
            Array.Copy(Data, 0, res, 8, Data.Length);
            return res;
        }

        public IMessage Parse()
        {
            var desc = MessageType.GetProperty("Descriptor").GetValue(null) as MessageDescriptor;
            Debug.Assert(desc != null);
            return desc.Parser.ParseFrom(Data);
        }
    }
}
