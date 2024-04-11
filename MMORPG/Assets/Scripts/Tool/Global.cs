
public class Logger
{
#if UNITY_EDITOR
    public void Info(object message)
    {
        UnityEngine.Debug.Log(message);
    }

    public void Debug(object message)
    {
        //UnityEngine.Debug.Log(message);
    }

    public void Error(object message)
    {
        UnityEngine.Debug.LogError(message);
    }

    public void Warn(object message)
    {
        UnityEngine.Debug.LogWarning(message);
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