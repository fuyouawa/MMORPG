//using Common.Network;
//using Common.Proto;
//using Common.Proto.User;
//using Common.Tool;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace Common.Network
//{
//    public class MessageRouter
//    {
//        public delegate void MessageHandler<TMessage>(object? sender, TMessage msg) where TMessage : Google.Protobuf.IMessage;

//        private Dictionary<Type, Delegate?> _messageHandlers = new Dictionary<Type, Delegate?>();
//        private MethodInfo _dispatchToHandlerMethod;

//        public MessageRouter() : base()
//        {
//            _dispatchToHandlerMethod = GetType().GetMethod("DispatchToHandler", BindingFlags.NonPublic | BindingFlags.Instance);
//        }

//        public void Reigster<TMessage>(MessageHandler<TMessage> handler) where TMessage: Google.Protobuf.IMessage
//        {
//            var type = typeof(TMessage);
//            if (!_messageHandlers.ContainsKey(type))
//                _messageHandlers.Add(type, null);
//            _messageHandlers[type] = (_messageHandlers[type] as MessageHandler<TMessage>) + handler;
//        }
//        public void UnReigster<TMessage>(MessageHandler<TMessage> handler) where TMessage : Google.Protobuf.IMessage
//        {
//            var type = typeof(TMessage);
//            Debug.Assert(_messageHandlers.ContainsKey(type));
//            _messageHandlers[type] = (_messageHandlers[type] as MessageHandler<TMessage>) - handler;
//        }

//        public void Dispatch(object? sender, Packet packet)
//        {
//            Task.Run(() =>
//            {
//                var method = _dispatchToHandlerMethod.MakeGenericMethod(packet.MessageType);
//                method.Invoke(this, new object[] { sender, packet.Parse() });
//            });
//        }

//        private void DispatchToHandler<TMessage>(object? sender, TMessage msg) where TMessage : Google.Protobuf.IMessage
//        {
//            var type = typeof(TMessage);
//            if (_messageHandlers.ContainsKey(type))
//            {
//                var handler = _messageHandlers[type] as MessageHandler<TMessage>;
//                try
//                {
//                    handler?.Invoke(sender, msg);
//                }
//                catch (Exception ex)
//                {

//                }
//            }
//        }
//    }
//}
