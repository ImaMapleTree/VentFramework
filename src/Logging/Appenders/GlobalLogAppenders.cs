using VentLib.Utilities.Collections;

namespace VentLib.Logging.Appenders;

public class GlobalLogAppenders
{
    internal static readonly RemoteList<ILogAppender> Appenders = new();

    public static Remote<ILogAppender> AddAppender(ILogAppender appender) => Appenders.Add(appender);
}