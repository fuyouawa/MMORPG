using Common.Tool;
using GameServer.Tool;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.System
{
    public class CenterTimer : Singleton<CenterTimer>
    {
        private TimeWheel _timer;

        private CenterTimer() 
        {
            _timer = new();
            _timer.Start();
        }

        public void Register(int ms, Action task)
        {
            _timer.AddTask(ms, (timeTask) =>
            {
                _timer.AddTask(ms, timeTask.Action);
                task();
            });
        }
    }

}
