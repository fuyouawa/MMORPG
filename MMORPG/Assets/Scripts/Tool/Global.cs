using UnityEngine;

public class Logger
{
#if UNITY_EDITOR
    public void Info(object message)
    {
        Debug.Log(message);
    }

    public void Error(object message)
    {
        Debug.LogError(message);
    }

    public void Warn(object message)
    {
        Debug.LogWarning(message);
    }
#else
    private NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

    public void Info(object message)
    {
        _logger.Info(message);
    }

    public void Error(object message)
    {
        _logger.Error(message);
    }

    public void Warn(object message)
    {
        _logger.Warn(message);
    }
#endif
}

public static class Global
{
    public static Logger Logger { get; private set; } = new Logger();
}