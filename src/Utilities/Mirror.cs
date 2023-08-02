using System.Diagnostics;
using System.Reflection;

namespace VentLib.Utilities;

public static class Mirror
{
    internal static MethodBase? GetCallerDebug()
    {
#if DEBUG // Stack trace is a bit costly so we'll only do this during debug
        return new StackTrace(2, false).GetFrame(0)?.GetMethod();
#endif
#if !DEBUG
        return null;
#endif
    }
    
    public static MethodBase? GetCaller()
    {
        return new StackTrace(2, false).GetFrame(0)?.GetMethod();
    }
}