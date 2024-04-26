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

    public static void Info(string whichModel, string message)
    {
        var msg = $"[{whichModel}] {message}";
        Log.Information(msg);
        QFramework.LogKit.I(msg);
    }

    public static void Warn(string whichModel, string message)
    {
        var msg = $"[{whichModel}] {message}";
        Log.Warning(msg);
        QFramework.LogKit.W(msg);
    }

    public static void Error(string whichModel, string message)
    {
        var msg = $"[{whichModel}] {message}";
        Log.Error(msg);
        QFramework.LogKit.E(msg);
    }

    public static void Error(string whichModel, Exception ex, string message)
    {
        var msg = $"[{whichModel}] {message}";
        Log.Error(ex, msg);
        QFramework.LogKit.E(msg);
    }
}
