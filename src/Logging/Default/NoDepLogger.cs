using System;
using System.Collections.Generic;
using System.Reflection;
using VentLib.Logging.Accumulators;
using VentLib.Logging.Appenders;
using VentLib.Utilities;
using VentLib.Utilities.Collections;

namespace VentLib.Logging.Default;

internal static class NoDepLogger
{
    private static readonly DefaultDictionary<Type, UninitializedLoggerDelegate> UnitLoggers = new(t => new UninitializedLoggerDelegate(t));
    
    private static readonly object[] NoArgs = Array.Empty<object>();
    internal static List<ILogAccumulator> UninitAccumulators = new()
    {
        new TimestampAccumulator(),
        new LogLevelAccumulator(charCase: LogLevelAccumulator.CharCase.AllUpper),
        new ConstantAccumulator("---"),
        new DelimitingAccumulator(Delimiter.BracketsPadded, new ColoringAccumulator(System.Drawing.Color.MediumVioletRed, new PaddingAccumulator(10, PaddingOption.PadRight, new ThreadAccumulator()))),
        new DelimitingAccumulator(new Delimiter("", ":"), new ColoringAccumulator(System.Drawing.Color.DarkCyan, new LoggerTypeAccumulator(40, PaddingOption.PadRight)))
    };

    public static void Log(LogLevel level, string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();

        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(level, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }
    
    public static void Trace(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();

        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Trace, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Debug(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Debug, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Info(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Info, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void High(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.High, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Warn(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Warn, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Warn(string message, Exception exception, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Warn, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod, exception));
    }

    public static void Warn(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Warn, exception.Message, LogArguments.Wrap(logger, NoArgs, callingAssembly, callerMethod, exception));
    }

    public static void Exception(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Error, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Exception(string message, Exception exception, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Error, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod, exception));
    }

    public static void Exception(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Error, exception.Message, LogArguments.Wrap(logger, NoArgs, callingAssembly, callerMethod, exception));
    }
    
    public static void Fatal(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Fatal, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Fatal(string message, Exception exception, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Fatal, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod, exception));
    }

    public static void Fatal(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Type callingType = callerMethod?.DeclaringType ?? typeof(StaticLogger);
        Logger logger = Logger.IsLoaded ? LoggerFactory.GetLogger(callingType) : UnitLoggers.Get(callingType);
        
        logger.Log(LogLevel.Fatal, exception.Message, LogArguments.Wrap(logger, NoArgs, callingAssembly, callerMethod, exception));
    }

    private class UninitializedLoggerDelegate: DefaultLogger
    {
        private static readonly ConsoleAppender ConsoleAppender = new(LogLevel.Trace);
    
        public UninitializedLoggerDelegate(Type declaringType)
        {
            DeclaringType = declaringType;
            Accumulators.Clear();
            Accumulators.AddRange(UninitAccumulators);
            Appenders.Add(ConsoleAppender);
        }
    }
}