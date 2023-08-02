using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP.Logging;
using VentLib.Logging.Accumulators;
using VentLib.Logging.Default;

namespace VentLib.Logging;

internal sealed class BepInExLogListener: ILogListener
{
    private static readonly Exception EmptyException = new();
    private static readonly object[] EmptyArray = Array.Empty<object>();
    
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(BepInExLogListener));
    
    public BepInEx.Logging.LogLevel LogLevelFilter { get; }

    public static void BindToUnity()
    {
        ConsoleLogListener consoleLog = (ConsoleLogListener)BepInEx.Logging.Logger.Listeners.First(l => l.GetType() == typeof(ConsoleLogListener));
        BepInExLogListener interceptor = new(consoleLog.LogLevelFilter);
        BepInEx.Logging.Logger.Listeners.Remove(consoleLog);
        BepInEx.Logging.Logger.Listeners.Add(interceptor);

        if (log is not DefaultLogger defaultLogger) return;
        
        ModifyAccumulator(defaultLogger.Accumulators);
    }
    

    private BepInExLogListener(BepInEx.Logging.LogLevel logLevelFilter)
    {
        LogLevelFilter = logLevelFilter;
    }
    
    public void Dispose()
    {
    }

    public void LogEvent(object sender, LogEventArgs eventArgs)
    {
        string message = eventArgs.Data.ToString() ?? "";
        if (sender is ManualLogSource)
        {
            ConsoleManager.ConsoleStream.WriteLine(message);
        }
        else
        {
            if (sender is not (IL2CPPLogSource or IL2CPPUnityLogSource)) LogUnityError(message);
            else log.Log(ConvertLevel(eventArgs.Level), message);
        }
    }

    private static void ModifyAccumulator(IEnumerable<ILogAccumulator> accumulators)
    {
        LoggerTypeAccumulator? typeAccumulator = accumulators.Select(FindTypeAccumulator).FirstOrDefault(ft => ft != null);
        if (typeAccumulator == null) return;
        
        typeAccumulator.FormattedTypes[typeof(BepInExLogListener)] = typeAccumulator.PadString("UNITY"); 
        
        return;

        LoggerTypeAccumulator? FindTypeAccumulator(ILogAccumulator accumulator)
        {
            if (accumulator is LoggerTypeAccumulator loggerTypeAccumulator) return loggerTypeAccumulator;
            if (accumulator is not IWrappingAccumulator wrappingAccumulator) return null;
            return wrappingAccumulator.WrappedAccumulators.Select(FindTypeAccumulator).FirstOrDefault(foundType => foundType != null);
        }
    }

    private static void LogUnityError(string error)
    {
        log.Log(LogLevel.Error, error, LogArguments.Wrap(log, EmptyArray, exception: EmptyException));
    }

    private static LogLevel ConvertLevel(BepInEx.Logging.LogLevel logLevel)
    {
        return logLevel switch
        {
            BepInEx.Logging.LogLevel.None => LogLevel.All,
            BepInEx.Logging.LogLevel.Fatal => LogLevel.Fatal,
            BepInEx.Logging.LogLevel.Error => LogLevel.Error,
            BepInEx.Logging.LogLevel.Warning => LogLevel.Warn,
            BepInEx.Logging.LogLevel.Message => LogLevel.High,
            BepInEx.Logging.LogLevel.Info => LogLevel.Info,
            BepInEx.Logging.LogLevel.Debug => LogLevel.Debug,
            BepInEx.Logging.LogLevel.All => LogLevel.All,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel), logLevel, null)
        };
    }
    
}