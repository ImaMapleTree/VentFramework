using System;
using System.Reflection;

namespace VentLib.Logging;

public class LogArguments
{
    public Logger Logger { get; }
    public Assembly? CallerAssembly { get; }
    public MethodBase? CallerMethod { get; }

    public object?[] Arguments { get; }
    public Exception? Exception { get; }
    public DateTime CreationTime { get; } = DateTime.Now;
    
    public LogArguments(Logger logger, Assembly? callerAssembly, MethodBase? callerMethod, object?[] arguments, Exception? exception = null)
    {
        Logger = logger;
        CallerAssembly = callerAssembly;
        CallerMethod = callerMethod;
        Arguments = arguments;
        Exception = exception;
    }

    public static LogArguments Wrap(Logger logger, object?[] arguments, Assembly? caller = null, MethodBase? callerMethod = null, Exception? exception = null)
    {
        return new LogArguments(logger, caller, callerMethod, arguments, exception);
    }
}