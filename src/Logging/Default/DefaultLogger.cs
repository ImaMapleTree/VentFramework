using System;
using System.Collections.Generic;
using VentLib.Logging.Appenders;
using VentLib.Utilities;

namespace VentLib.Logging.Default;

public class DefaultLogger: StandardLogger
{
    internal static DefaultLogger BaseLogger = new();
    private static readonly object?[] NoArgs = Array.Empty<object>();
    
    public Type? DeclaringType { get; protected set; }
    
    public Delimiter Delimiter { get; set; }
    public readonly List<ILogAppender> Appenders;
    public readonly List<ILogAccumulator> Accumulators;

    internal DefaultLogger()
    {
        DeclaringType = null;
        Delimiter = Delimiter.Space;
        Appenders = new List<ILogAppender>();
        Accumulators = new List<ILogAccumulator>();
    }
    
    protected DefaultLogger(Type declaringType, List<ILogAccumulator> accumulators, List<ILogAppender> appenders, Delimiter delimiter)
    {
        this.DeclaringType = declaringType;
        this.Accumulators = accumulators;
        Appenders = appenders;
        Delimiter = delimiter;
    }
    
    public void Log(LogLevel level, string message, LogArguments arguments)
    {
        LogComposite composite = new LogCompositeStd(level, message, Delimiter);

        foreach (ILogAccumulator accumulator in Accumulators)
        {
            composite = accumulator.Accumulate(composite, arguments);
        }

        foreach (ILogAppender appender in GlobalLogAppenders.Appenders)
        {
            appender.Receive(composite, arguments);
        }

        foreach (ILogAppender appender in Appenders)
        {
            appender.Receive(composite, arguments);
        }
    }

    public virtual void Trace(string message, params object?[] args)
    {
        Log(LogLevel.Trace, message, LogArguments.Wrap(this, args));
    }

    public virtual void Debug(string message, params object?[] args)
    {
        Log(LogLevel.Debug, message, LogArguments.Wrap(this, args));
    }

    public virtual void Info(string message, params object?[] args)
    {
        Log(LogLevel.Info, message, LogArguments.Wrap(this, args));
    }

    public virtual void High(string message, params object?[] args)
    {
        Log(LogLevel.High, message, LogArguments.Wrap(this, args));
    }

    public virtual void Warn(string message, params object?[] args)
    {
        Log(LogLevel.Warn, message, LogArguments.Wrap(this, args));
    }

    public virtual void Warn(string message, Exception exception, params object?[] args)
    {
        Log(LogLevel.Warn, message, LogArguments.Wrap(this, args, exception: exception));
    }

    public virtual void Warn(Exception exception)
    {
        Log(LogLevel.Warn, exception.Message, LogArguments.Wrap(this, NoArgs, exception: exception));
    }

    public virtual void Exception(string message, params object?[] args)
    {
        Log(LogLevel.Error, message, LogArguments.Wrap(this, args));
    }

    public virtual void Exception(string message, Exception exception, params object?[] args)
    {
        Log(LogLevel.Error, message, LogArguments.Wrap(this, args, exception: exception));
    }

    public virtual void Exception(Exception exception)
    {
        Log(LogLevel.Error, exception.Message, LogArguments.Wrap(this, NoArgs, exception: exception));
    }
    
    public virtual void Fatal(string message, params object?[] args)
    {
        Log(LogLevel.Fatal, message, LogArguments.Wrap(this, args));
    }

    public virtual void Fatal(string message, Exception exception, params object?[] args)
    {
        Log(LogLevel.Fatal, message, LogArguments.Wrap(this, args, exception: exception));
    }

    public virtual void Fatal(Exception exception)
    {
        Log(LogLevel.Fatal, exception.Message, LogArguments.Wrap(this, NoArgs, exception: exception));
    }

    public virtual DefaultLogger Create(Type declaringType, List<ILogAccumulator> accumulators, List<ILogAppender> appenders, Delimiter delimiter)
    {
        return new DefaultLogger(declaringType, accumulators, appenders, delimiter);
    }
}