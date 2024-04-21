using Common.Network;
using Common.Proto.Player;
using Common.Tool;
using Google.Protobuf;
using QFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;

namespace MMORPG
{
    public interface INetworkSystem : ISystem
    {
        public void RegisterEmergencyReceive<TMessage>(Action<TMessage> onReceived) where TMessage : class, IMessage;
        public void UnregisterEmergencyReceive<TMessage>(Action<TMessage> onReceived) where TMessage : class, IMessage;

        public Task ConnectAsync();
        public void SendToServer(IMessage msg);
        public Task<T> ReceiveAsync<T>() where T : class, IMessage;
        public Task StartAsync();
    }

    public class NetworkSystem : AbstractSystem, INetworkSystem
    {
        private NetSession _session;
        private Dictionary<Type, Delegate> _emergencyHandlers = new();

        ////TODO 高水位处理
        private LinkedList<IMessage> _messageList = new();

        public async Task<T> ReceiveAsync<T>() where T : class, IMessage
        {
            while (true)
            {
                IMessage msg = null;
                lock (_messageList)
                {
                    var node = _messageList.FindIf(msg => { return msg.GetType() == typeof(T); });
                    if (node != null)
                    {
                        msg = node.Value;
                        //UnityEngine.Debug.Log(typeof(T));
                        _messageList.Remove(node);
                    }
                }
                if (msg == null)
                {
                    await Task.Delay(100);
                    continue;
                }
                var res = msg as T;
                Debug.Assert(res != null);
                return res;
            }
        }

        public void SendToServer(IMessage msg)
        {
            _session.Send(msg);
        }

        public Task StartAsync()
        {
            _session.PacketReceived += OnPacketReceived;
            return _session.StartAsync();
        }

        private void OnPacketReceived(object sender, Common.Network.PacketReceivedEventArgs e)
        {
            var msgType = e.Packet.Message.GetType();
            if (ProtoManager.IsEmergency(msgType))
            {
                _emergencyHandlers[msgType]?.DynamicInvoke(new object[] { e.Packet.Message });
            }
            else
            {
                _messageList.AddLast(e.Packet.Message);
            }
        }

        public async Task ConnectAsync()
        {
            Socket socket;
            var box = this.GetSystem<IBoxSystem>();
            while (true)
            {
                // 显示旋转加载框
                box.ShowSpinner("连接服务器中......");
                try
                {
                    socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    await socket.ConnectAsync(NetConfig.ServerIpAddress, NetConfig.ServerPort);
                    box.CloseSpinner();
                    break;
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, $"[Network]连接服务器时出现错误:{ex.Message}");
                    box.CloseSpinner();
                    await box.ShowMessageAsync("错误", $"连接服务器失败:{ex}", "重新连接");
                    continue;
                }
            }
            _session = new NetSession(socket);
        }

        protected override void OnInit()
        {
        }

        void INetworkSystem.RegisterEmergencyReceive<TMessage>(Action<TMessage> onReceived)
        {
            var type = typeof(TMessage);
            if (!_emergencyHandlers.ContainsKey(type))
            {
                _emergencyHandlers[type] = null;
            }
            _emergencyHandlers[type] = (_emergencyHandlers[type] as Action<TMessage>) + onReceived;
        }

        void INetworkSystem.UnregisterEmergencyReceive<TMessage>(Action<TMessage> onReceived)
        {
            var type = typeof(TMessage);
            Debug.Assert(_emergencyHandlers.ContainsKey(type));
            _emergencyHandlers[type] = (_emergencyHandlers[type] as Action<TMessage>) - onReceived;
        }
    }
}