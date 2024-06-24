using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameServer.Tool
{
    public static class Time
    {
        private static long _startTime = 0;
        private static long _lastTime = 0;

        //TODO time命名
        public static float time { get; private set; }
        public static float DeltaTime { get; private set; }

        public static void Tick()
        {
            var now = DateTimeOffset.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            if (_startTime == 0) _startTime = now;
            time = (now - _startTime) * 0.001f;
            if (_lastTime == 0) _lastTime = now;
            DeltaTime = (now - _lastTime) * 0.001f;
            //Log.Debug($"Tick：{time}，{DeltaTime}");
            _lastTime = now;
        }
    }
}
