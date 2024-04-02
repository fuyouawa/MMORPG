using Common.Network;
using Common.Proto;
using Common.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    public class MessageRouter : Singleton<MessageRouter>
    {
        public delegate void MessageHandler<TMessage>(object? sender, TMessage msg) where TMessage : Google.Protobuf.IMessage;

        //private record SessionPacket(Connection Connection, BytesPacket Packet);

        //private Queue<SessionPacket> _pendingDispatchQueue = new();
        private Dictionary<Type, Delegate?> _messageHandlers = new Dictionary<Type, Delegate?>();

        private MethodInfo DispatchToHandlerMethod { get; }

        public MessageRouter() : base()
        {
            DispatchToHandlerMethod = GetType().GetMethod("DispatchToHandler", BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public void Reigster<TMessage>(MessageHandler<TMessage> handler) where TMessage: Google.Protobuf.IMessage
        {
            var type = typeof(TMessage);
            if (!_messageHandlers.ContainsKey(type))
                _messageHandlers.Add(type, null);
            _messageHandlers[type] = (_messageHandlers[type] as MessageHandler<TMessage>) + handler;
        }
        public void UnReigster<TMessage>(MessageHandler<TMessage> handler) where TMessage : Google.Protobuf.IMessage
        {
            var type = typeof(TMessage);
            Debug.Assert(_messageHandlers.ContainsKey(type));
            _messageHandlers[type] = (_messageHandlers[type] as MessageHandler<TMessage>) - handler;
        }

        public void DispatchMessage(object? sender, NetMessage msg)
        {
            Debug.Assert(Alogrithm.IsUniqueNull(msg.Request, msg.Response));

            Task.Run(() =>
            {
                var realyMsg = (msg.Request != null ? msg.Request : msg.Response as object);
                foreach (var property in realyMsg.GetType().GetProperties())
                {
                    if (property == null)
                        continue;
                    var value = property.GetValue(realyMsg);
                    if (value == null)
                        continue;
                    var valueType = value.GetType();
                    if (typeof(Google.Protobuf.IMessage).IsAssignableFrom(valueType))
                    {
                        var method = DispatchToHandlerMethod.MakeGenericMethod(valueType);
                        method.Invoke(this, new object[] { sender, value });
                    }
                }
            });
        }

        private void DispatchToHandler<TMessage>(object? sender, TMessage msg) where TMessage : Google.Protobuf.IMessage
        {
            var type = typeof(TMessage);
            if (_messageHandlers.ContainsKey(type))
            {
                var handler = _messageHandlers[type] as MessageHandler<TMessage>;
                try
                {
                    handler?.Invoke(sender, msg);
                }
                catch (Exception ex)
                {

                }
            }
        }
    }
}
