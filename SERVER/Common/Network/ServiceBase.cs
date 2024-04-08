using Common.Tool;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Common.Network
{
    public class ServiceBase<T> where T : ServiceBase<T>, new()
    {
        private static T _instance;
        public static T Instance => _instance ??= new T();

        private Dictionary<Type, MethodInfo> _handlers;

        protected ServiceBase()
        {
            _handlers = (from m in GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance)
                         where m.Name == "OnHandle"
                         select m)
                         .ToDictionary(m => m.GetParameters()[1].ParameterType, m => m);
        }

        public bool HandleMessage<TConnection>(TConnection sender, Google.Protobuf.IMessage msg)
            where TConnection : Connection
        {
            var msgType = msg.GetType();
            if (_handlers.ContainsKey(msgType))
            {
                _handlers[msgType].Invoke(this, new object[] { sender, msg });
                return true;
            }
            return false;
        }
    }
}
