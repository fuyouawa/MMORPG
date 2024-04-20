using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class Logger
{
    public static void Initialize()
    {
        Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Async(a => a.File("Logs/log-.txt", rollingInterval: RollingInterval.Day))
                .CreateLogger();
    }

    public static void Info(string message)
    {
        Log.Information(message);
        QFramework.LogKit.I(message);
    }

    public static void Warn(string message)
    {
        Log.Warning(message);
        QFramework.LogKit.W(message);
    }

    public static void Error(string message)
    {
        Log.Error(message);
        QFramework.LogKit.E(message);
    }

    public static void Error(Exception ex, string message)
    {
        Log.Error(ex, message);
        QFramework.LogKit.E(message);
    }
}