namespace VentLib.Logging.Accumulators;

public class CallerMethodAccumulator: ILogAccumulator
{
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        return composite.Append(arguments.CallerMethod?.Name ?? "N/A");
    }
}