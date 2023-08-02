using System;
using System.Collections.Generic;
using VentLib.Logging.Finalizers;

namespace VentLib.Logging;

public interface ILoggerFactory
{
    List<ILogCompleter> Completers { get; }
    
    public T GetLogger<T>(Type cls) where T : Logger;

    public Logger GetLogger(Type cls);
}