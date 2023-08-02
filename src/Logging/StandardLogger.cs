using System;

namespace VentLib.Logging;

// ReSharper disable once InconsistentNaming
public interface StandardLogger: Logger
{
    void Trace(string message, params object?[] args);

    void Debug(string message, params object?[] args);

    void Info(string message, params object?[] args);

    void High(string message, params object?[] args);

    void Warn(string message, params object?[] args);

    void Warn(string message, Exception exception, params object?[] args);

    void Warn(Exception exception);

    void Exception(string message, params object?[] args);

    void Exception(string message, Exception exception, params object?[] args);

    void Exception(Exception exception);
    
    void Fatal(string message, params object?[] args);

    void Fatal(string message, Exception exception, params object?[] args);

    void Fatal(Exception exception);
}