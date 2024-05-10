using System;
using Serilog;

namespace MMORPG.Tool
{
    public static class Log
    {
        public static void Initialize()
        {
            Serilog.Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();
        }

        public static void Info(string whichModel, string message)
        {
            var msg = $"[{whichModel}] {message}";
            Serilog.Log.Information(msg);
            QFramework.LogKit.I(msg);
        }

        public static void Warn(string whichModel, string message)
        {
            var msg = $"[{whichModel}] {message}";
            Serilog.Log.Warning(msg);
            QFramework.LogKit.W(msg);
        }

        public static void Error(string whichModel, string message)
        {
            var msg = $"[{whichModel}] {message}";
            Serilog.Log.Error(msg);
            QFramework.LogKit.E(msg);
        }

        public static void Error(string whichModel, Exception ex, string message)
        {
            var msg = $"[{whichModel}] {message}";
            Serilog.Log.Error(ex, msg);
            QFramework.LogKit.E(msg);
        }
    }
}
