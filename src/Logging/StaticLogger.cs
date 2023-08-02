using System;
using System.Reflection;
using VentLib.Utilities;

namespace VentLib.Logging;

public class StaticLogger
{
    private static readonly object[] NoArgs = Array.Empty<object>();

    public static void Log(LogLevel level, string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();

        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(level, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }
    
    public static void Trace(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();

        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Trace, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Debug(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Debug, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Info(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Info, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void High(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.High, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Warn(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Warn, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Warn(string message, Exception exception, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Warn, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod, exception));
    }

    public static void Warn(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Warn, exception.Message, LogArguments.Wrap(logger, NoArgs, callingAssembly, callerMethod, exception));
    }

    public static void Exception(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Error, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Exception(string message, Exception exception, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Error, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod, exception));
    }

    public static void Exception(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Error, exception.Message, LogArguments.Wrap(logger, NoArgs, callingAssembly, callerMethod, exception));
    }
    
    public static void Fatal(string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Fatal, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod));
    }

    public static void Fatal(string message, Exception exception, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Fatal, message, LogArguments.Wrap(logger, args, callingAssembly, callerMethod, exception));
    }

    public static void Fatal(Exception exception)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        
        Logger logger = LoggerFactory.GetLogger(callerMethod?.DeclaringType ?? typeof(StaticLogger));
        logger.Log(LogLevel.Fatal, exception.Message, LogArguments.Wrap(logger, NoArgs, callingAssembly, callerMethod, exception));
    }
}