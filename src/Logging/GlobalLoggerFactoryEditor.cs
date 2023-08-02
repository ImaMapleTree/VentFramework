namespace VentLib.Logging;

public static class GlobalLoggerFactoryEditor
{
    public static void SetDefaultLoggerFactory(ILoggerFactory loggerFactory)
    {
        LoggerFactory.FactoryInstance = loggerFactory;
    }
}