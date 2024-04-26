using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    public class Time
    {
        private long _startTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        private long _lastTick = 0;

        //TODO time命名
        public float time { get; private set; }
        public float DeltaTime { get; private set; }

        public void Tick()
        {
            var now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            time = (now - _startTime) * 0.001f;
            if (_lastTick == 0) _lastTick = now;
            DeltaTime = (now - _lastTick) * 0.001f;
            _lastTick = now;
        }
    }
}
