using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    public class Time
    {
        private long startTime = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
        private long _lastTick = 0;

        public float time { get; private set; }
        public float deltaTime { get; private set; }

        public void Tick()
        {
            long now = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            time = (now - startTime) * 0.001f;
            if (_lastTick == 0) _lastTick = now;
            deltaTime = (now - _lastTick) * 0.001f;
            _lastTick = now;
        }
    }
}
