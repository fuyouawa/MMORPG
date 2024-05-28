using GameServer.System;
using GameServer.Tool;
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

        private UpdateManager() { }

        public void Init()
        {
            Scheduler.Instance.Register(1000 / Fps, Update);
        }

        public void Update()
        {
            Time.Tick();
            EntityManager.Instance.Update();
            MapManager.Instance.Update();
        }
    }
}
