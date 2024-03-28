using Common;
using Proto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Common.Network
{
    class Msg
    {
        public Connection sender;
        public Proto.Package message;
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
        private Queue<Msg> messageQueue = new Queue<Msg>();

        /// <summary>
        /// 消息处理器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="msg"></param>
        public delegate void MessageHandler<T>(Connection sender, T msg);

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
            string type = typeof(T).FullName;
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
            string type = typeof(T).FullName;
            if (!delegateMap.ContainsKey(type))
            {
                delegateMap[type] = null;
            }
            var _handler =
            delegateMap[type] = (delegateMap[type] as MessageHandler<T>) - handler;
        }

        private void Fire<T>(Connection sender, T msg) where T : Google.Protobuf.IMessage
        {
            string type = typeof(T).FullName;
            if (delegateMap.ContainsKey(type))
            {
                MessageHandler<T> handler = delegateMap[type] as MessageHandler<T>;
                try
                {
                    handler?.Invoke(sender, msg);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("MessageRouter.Fire error:" + ex.StackTrace);
                }
            }
        }

        /// <summary>
        /// 添加新的消息到队列中
        /// </summary>
        /// <param name="sender">消息发送者</param>
        /// <param name="message">消息对象</param>
        public void AddMessage(Connection sender, Proto.Package message)
        {
            messageQueue.Enqueue(new Msg() { sender=sender, message= message });
            threadEvent.Set();
        }

        public void Start(int threadCount)
        {
            running = true;
            this.threadCount = Math.Max(1, threadCount);
            this.threadCount = Math.Max(200, this.threadCount);
            for (int i = 0; i < threadCount; i++)
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
        }

        private void MessageWork(object? state)
        {
            try
            {
                Console.WriteLine("工作线程启动");
                this.workCount = Interlocked.Increment(ref this.workCount);
                while (running)
                {
                    if (messageQueue.Count == 0)
                    {
                        threadEvent.WaitOne();
                        continue;
                    }
                    Msg msg = messageQueue.Dequeue();
                    Proto.Package package = msg.message;
                    if (package != null)
                    {
                        if (package.Request != null)
                        {
                            execute(msg.sender, package.Request);
                        }
                        if (package.Response != null)
                        {
                            execute(msg.sender, package.Response);
                        }
                    }
                }
            }
            catch (Exception ex) {
                Console.WriteLine(ex.StackTrace);
            }
            finally
            {
                Console.WriteLine("工作线程退出");
                this.workCount = Interlocked.Decrement(ref this.workCount);
            }
        }

        /// <summary>
        /// 根据反射原理对消息进行自动分发
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="entity"></param>
        private void execute(Connection sender, object entity)
        {
            var fireMethod = this.GetType().GetMethod("Fire",
                BindingFlags.NonPublic | BindingFlags.Instance);
            Type t = entity.GetType();
            foreach (var p in t.GetProperties())
            {
                if (p.Name == "Parser" || p.Name == "Descriptor") continue;
                var value = p.GetValue(entity);
                if (value != null)
                {
                    var met = fireMethod.MakeGenericMethod(value.GetType());
                    met.Invoke(this, new object[] { sender, value });
                }
            }
        }
    }
}
