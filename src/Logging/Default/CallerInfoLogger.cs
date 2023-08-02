using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using VentLib.Logging.Accumulators;
using VentLib.Logging.Appenders;
using VentLib.Utilities;

namespace VentLib.Logging.Default;

internal class CallerInfoLogger: DefaultLogger
{
    internal static List<ILogAccumulator> CallerInfoAccumulators = new()
    {
        new TimestampAccumulator(),
        new LogLevelAccumulator(charCase: LogLevelAccumulator.CharCase.AllUpper),
        new DelimitingAccumulator(new Delimiter("", "|"), new ColoringAccumulator(Color.MediumVioletRed, new PaddingAccumulator(3, PaddingOption.PadRight, new ThreadAccumulator(ThreadAccumulator.ThreadDisplay.ID)))),
        new CustomCompositeAppendAccumulator(ChangeUnityColor),
        new ColoringAccumulator(Color.DarkCyan, new PaddingAccumulator(15, PaddingOption.PadRight, new LoggerTypeAccumulator(qualifierDepth: 1))),
        new ColoringAccumulator(Color.FromArgb(53, 128, 114), new PaddingAccumulator(15, PaddingOption.PadRight, new CustomCompositeAppendAccumulator(RemoveUnityMethods), new CallerMethodAccumulator())),
        new ConstantAccumulator(":")
    };
    
    internal new static CallerInfoLogger BaseLogger = new();
    private static readonly object?[] NoArgs = Array.Empty<object>();

    private CallerInfoLogger()
    {
    }
    
    protected CallerInfoLogger(Type declaringType, List<ILogAccumulator> accumulators, List<ILogAppender> appenders, Delimiter delimiter) : base(declaringType, accumulators, appenders, delimiter)
    {
    }

    public override void Trace(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Trace, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void Debug(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Debug, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void Info(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Info, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void High(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.High, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void Warn(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Warn, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void Warn(string message, Exception exception, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Warn, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod, exception));
    }

    public override void Warn(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Warn, exception.Message, LogArguments.Wrap(this, NoArgs, callingAssembly, callerMethod, exception));
    }

    public override void Exception(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Error, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void Exception(string message, Exception exception, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Error, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod, exception));
    }

    public override void Exception(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Error, exception.Message, LogArguments.Wrap(this, NoArgs, callingAssembly, callerMethod, exception));
    }
    
    public override void Fatal(string message, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Fatal, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    public override void Fatal(string message, Exception exception, params object?[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Fatal, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod, exception));
    }

    public override void Fatal(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(LogLevel.Fatal, exception.Message, LogArguments.Wrap(this, NoArgs, callingAssembly, callerMethod, exception));
    }

    public override DefaultLogger Create(Type declaringType, List<ILogAccumulator> accumulators, List<ILogAppender> appenders, Delimiter delimiter)
    {
        return new CallerInfoLogger(declaringType, accumulators, appenders, delimiter);
    }

    private static void ChangeUnityColor(LogCompositeEmitter.SegmentAppendEventArgs eventArgs)
    {
        if (eventArgs.Segment.Text.StartsWith("UNITY")) eventArgs.UpdateColor(_ => Color.FromArgb(87, 235, 155));
    }
    
    private static void RemoveUnityMethods(LogCompositeEmitter.SegmentAppendEventArgs eventArgs)
    {
        if (eventArgs.Segment.Text is "LogEvent") eventArgs.UpdateText(t => new string(' ', t.Length));
    }
}