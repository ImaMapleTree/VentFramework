namespace VentLib.Logging.Finalizers;

public interface ILogCompleter
{
    public void Complete(Logger logger, ILoggerFactory? factory);
}