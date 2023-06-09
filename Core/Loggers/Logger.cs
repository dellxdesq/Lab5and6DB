namespace Core.Loggers;

public class Logger : ILogger
{
    private static Logger _instance = new Logger();
    public static List<string> messages = new List<string>();
    
    private Logger()
    {
    }

    public static ILogger GetInstance()
    {
        return _instance;
    }

    public void Log(string message)
    {
        messages.Add(message);
    }
}