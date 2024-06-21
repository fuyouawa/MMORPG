using GameServer.EntitySystem;
using GameServer.MapSystem;
using GameServer.System;
using GameServer.Tool;
using GameServer.UserSystem;
using MMORPG.Common.Proto.Fight;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Manager
{
    /// <summary>
    /// 负责所有组件的更新
    /// </summary>
    public class UpdateManager : Singleton<UpdateManager>
    {
        public readonly int Fps = 10;
        private Queue<Action> _taskQueue = new();
        private Queue<Action> _backupTaskQueue = new();

        private UpdateManager()
        {
        }

        public void Start()
        {
            Scheduler.Instance.Register(1000 / Fps, Update);

            DataManager.Instance.Start();
            Log.Information("[Server] DataManager初始化完成");

            EntityManager.Instance.Start();
            Log.Information("[Server] EntityManager初始化完成");

            MapManager.Instance.Start();
            Log.Information("[Server] MapManager初始化完成");

            UserManager.Instance.Start();
            Log.Information("[Server] UserManager初始化完成");
        }

        public void Update()
        {
            Time.Tick();

            lock (_taskQueue)
            {
                (_backupTaskQueue, _taskQueue) = (_taskQueue, _backupTaskQueue);
            }

            foreach (var task in _backupTaskQueue)
            {
                task();
            }

            DataManager.Instance.Update();
            EntityManager.Instance.Update();
            MapManager.Instance.Update();
            UserManager.Instance.Update();
        }

        /// <summary>
        /// 线程安全
        /// </summary>
        /// <param name="task"></param>
        public void AddTask(Action task)
        {
            lock (_taskQueue)
            {
                _taskQueue.Enqueue(task);
            }
        }
    }
}
