using System;
using System.Reflection;
using VentLib.Utilities;

namespace VentLib.Logging;

// ReSharper disable once InconsistentNaming
public interface Logger
{
    internal static bool IsLoaded;
    
    public Type? DeclaringType { get; }
    
    void Log(LogLevel level, string message, params object[] args)
    {
        MethodBase? callerMethod = Mirror.GetCaller();
        Assembly callingAssembly = Assembly.GetCallingAssembly();
        Log(level, message, LogArguments.Wrap(this, args, callingAssembly, callerMethod));
    }

    void Log(LogLevel level, string message, LogArguments arguments);
}