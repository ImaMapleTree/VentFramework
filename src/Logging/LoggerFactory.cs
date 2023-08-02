using System;
using VentLib.Logging.Default;
using VentLib.Utilities;

namespace VentLib.Logging;

public static class LoggerFactory
{
    static LoggerFactory()
    {
        FactoryInstance = new DefaultLoggerFactory();
        Logger.IsLoaded = true;
    }
    
    internal static ILoggerFactory FactoryInstance;

    public static T GetLogger<T>(Type cls) where T : Logger => FactoryInstance.GetLogger<T>(cls);

    public static Logger GetLogger(Type cls) => FactoryInstance.GetLogger(cls);
    
    public static Logger GetLogger()
    {
        Type? callingType = Mirror.GetCaller()?.DeclaringType;
        if (callingType == null) throw new NullReferenceException("Could not get calling type for Logger.");
        return FactoryInstance.GetLogger(callingType);
    }
    
    public static T GetLogger<T>() where T: Logger
    {
        Type? callingType = Mirror.GetCaller()?.DeclaringType;
        if (callingType == null) throw new NullReferenceException("Could not get calling type for Logger.");
        return FactoryInstance.GetLogger<T>(callingType);
    }
}