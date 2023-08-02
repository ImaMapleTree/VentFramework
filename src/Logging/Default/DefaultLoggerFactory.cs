using System;
using System.Collections.Generic;
using System.Linq;
using VentLib.Logging.Accumulators;
using VentLib.Logging.Appenders;
using VentLib.Logging.Finalizers;
using VentLib.Utilities;
using VentLib.Utilities.Collections;

namespace VentLib.Logging.Default;

internal class DefaultLoggerFactory: ILoggerFactory
{
    public Delimiter Delimiter { get; set; } = Delimiter.Space;
    public List<ILogCompleter> Completers { get; } = new();

    public DefaultLogConfig DefaultConfig { get; } = new();
    
    private readonly DefaultDictionary<Type, Logger> loggers;
    private readonly List<ILogAccumulator> requiredAccumulators = new()
    {
        new ArgumentAccumulator(),
        new ColorMessageAccumulator()
    };
    
    private readonly List<ILogAppender> logAppenders = new();
    
    public DefaultLoggerFactory()
    {
        loggers = new DefaultDictionary<Type, Logger>(ConstructLogger);
        logAppenders.Add(new ConsoleAppender(DefaultConfig.ConsoleLevel));

        if (DefaultConfig.FileConfig.Enabled)
        {
            FlushingMemoryAppender memoryAppender = (FlushingMemoryAppender)DefaultConfig.FileConfig.CreateAppender();
            logAppenders.Add(memoryAppender);
            memoryAppender.CreateNewFile();
        }
        
        if (DefaultConfig.SupplyCallerInfo) 
            DefaultConfig.DefaultAccumulators = new List<ILogAccumulator>(CallerInfoLogger.CallerInfoAccumulators);
    }
    
    public T GetLogger<T>(Type cls) where T : Logger => (T)loggers.Get(cls);

    public Logger GetLogger(Type cls) => loggers.Get(cls);

    public void AddAppender(ILogAppender appender) => logAppenders.Add(appender);
    public void RemoveAppender(ILogAppender appender) => logAppenders.Remove(appender);

    private Logger ConstructLogger(Type cls)
    {
        List<ILogAppender> appenders = new(logAppenders);
        List<ILogAccumulator> accumulators = new(requiredAccumulators.Concat(DefaultConfig.DefaultAccumulators));
        
        DefaultLogger ctor = DefaultConfig.SupplyCallerInfo ? CallerInfoLogger.BaseLogger : DefaultLogger.BaseLogger;
        Logger logger = ctor.Create(cls, accumulators, appenders, Delimiter);
        foreach (ILogCompleter completer in Completers)
        {
            if (completer is IUnsafeLogCompleter unsafeCompleter) unsafeCompleter.Complete(ref logger, this);
            else completer.Complete(logger, this);
        }

        return logger;
    }
}