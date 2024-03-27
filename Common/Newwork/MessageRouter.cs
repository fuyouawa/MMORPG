using Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    class MsgUnit
    {
        public NetConnection sender;
        public Google.Protobuf.IMessage message;
    }
    /// <summary>
    /// 消息分发器
    /// </summary>
    public class MessageRouter : Singleton<MessageRouter>
    {
        int threadCount = 8;
        int workCount = 0;
        bool running = false;   // 是否正在运行
        AutoResetEvent threadEvent = new AutoResetEvent(true);

        /// <summary>
        /// 消息队列，所有客户端发来的消息都暂存在这里
        /// </summary>
        private Queue<MsgUnit> messageQueue = new Queue<MsgUnit>();

        /// <summary>
        /// 消息处理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public delegate void MessageHandler<T>(NetConnection sender, T msg);

        /// <summary>
        /// 消息频道
        /// </summary>
        private Dictionary<string, Delegate> delegateMap = new Dictionary<string, Delegate>();

        /// <summary>
        /// 订阅
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void On<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string type = typeof(T).Name;
            if(!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            var _handler = 
            delegateMap[type] = (delegateMap[type] as MessageHandler<T>) + handler;
        }

        /// <summary>
        /// 退订
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="handler"></param>
        public void Off<T>(MessageHandler<T> handler) where T : Google.Protobuf.IMessage
        {
            string type = typeof(T).Name;
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            var _handler =
            delegateMap[type] = (delegateMap[type] as MessageHandler<T>) - handler;
        }

        /// <summary>
        /// 添加新的消息到队列中
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">消息对象</param>
        public void AddMessage(NetConnection sender, Google.Protobuf.IMessage message)
        {
            messageQueue.Enqueue(new MsgUnit() { sender=sender, message= message });
        }

        public void Start(int threadCount)
        {
            running = true;
            this.threadCount = Math.Max(1, threadCount);
            this.threadCount = Math.Max(200, this.threadCount);
            for(int i = 0; i < threadCount; i++)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(MessageWork));
            }
            while (this.workCount < this.threadCount)
            {
                Thread.Sleep(100);
            }
        }

        public void Stop()
        {
            running = false;
            while (workCount > 0)
            {
                threadEvent.Set();
            }
            Thread.Sleep(50);
        }

        private void MessageWork(object? state)
        {
            try
            {
                this.workCount = Interlocked.Increment(ref this.workCount);
                while (running)
                {
                    if (messageQueue.Count == 0)
                    {
                        threadEvent.WaitOne();
                        continue;
                    }
                    MsgUnit pack = messageQueue.Dequeue();
                }
            }
            catch { }
            finally
            {
                this.workCount = Interlocked.Decrement(ref this.workCount);
            }
        }
    }
}
