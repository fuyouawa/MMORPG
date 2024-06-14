using Google.Protobuf;
using Google.Protobuf.Reflection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using MMORPG.Common.Tool;

namespace MMORPG.Common.Network
{
    public class Packet
    {
        private IMessage? _message;
        public IMessage Message
        {
            get
            {
                if (_message == null)
                {
                    var msgType = ProtoManager.IDToType(MessageID);
                    var desc = msgType.GetProperty("Descriptor").GetValue(null) as MessageDescriptor;
                    Debug.Assert(desc != null);
                    _message = desc.Parser.ParseFrom(Data);
                }
                return _message;
            }
        }

        public int MessageID { get; }
        public byte[] Data { get; }

        public Packet(IMessage msg)
        {
            MessageID = ProtoManager.TypeToID(msg.GetType());
            _message = msg;
            Data = msg.ToByteArray();
        }
        public Packet(int msgID, byte[] buffer)
        {
            MessageID = msgID;
            Data = buffer;
        }

        public byte[] Pack()
        {
            var lengthBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(Data.Length + NetConfig.PacketHeaderSize));
            var idBytes = BitConverter.GetBytes(IPAddress.HostToNetworkOrder(MessageID));
            var res = new byte[8 + Data.Length];
            Array.Copy(lengthBytes, res, 4);
            Array.Copy(idBytes, 0, res, 4, 4);
            Array.Copy(Data, 0, res, 8, Data.Length);
            return res;
        }
    }
}
