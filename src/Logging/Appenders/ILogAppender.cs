namespace VentLib.Logging.Appenders;

public interface ILogAppender
{
    public void Receive(LogComposite composite, LogArguments arguments);
}