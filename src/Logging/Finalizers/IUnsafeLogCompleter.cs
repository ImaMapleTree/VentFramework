namespace VentLib.Logging.Finalizers;

public interface IUnsafeLogCompleter: ILogCompleter
{
    void Complete(ref Logger logger, ILoggerFactory? factory);

    void ILogCompleter.Complete(Logger logger, ILoggerFactory? factory)
    {
        Complete(ref logger, factory);
    }
}