namespace VentLib.Logging;

public interface ILogAccumulator
{
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments);
}