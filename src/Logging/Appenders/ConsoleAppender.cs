namespace VentLib.Logging.Appenders;

public class ConsoleAppender: ILogAppender
{
    public bool Colored { get; set; } = true;

    public LogLevel MinimumLevel { get; }
    
    public ConsoleAppender(LogLevel minimumLevel)
    {
        MinimumLevel = minimumLevel;
    }
    
    public void Receive(LogComposite composite, LogArguments arguments)
    {
        if (composite.Level.Level < MinimumLevel.Level) return;
        System.Console.Out.WriteLine(composite.ToString(Colored));
    }
}