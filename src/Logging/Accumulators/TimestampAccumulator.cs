using System.Data;
using VentLib.Utilities;

namespace VentLib.Logging.Accumulators;

public class TimestampAccumulator : ILogAccumulator
{
    private string Format { get; set; }

    private Delimiter Delimiter
    {
        get => delimiter;
        set => delimiter = ValidateDelimiter(value);
    }

    private Delimiter delimiter = null!;

    public TimestampAccumulator(string format = "hh:mm:ss.fff", Delimiter? delimiter = null)
    {
        Delimiter = delimiter ?? Delimiter.Brackets;
        Format = format;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        return composite.Append(Delimiter.Enclose(arguments.CreationTime.ToString(Format)));
    }

    private static Delimiter ValidateDelimiter(Delimiter delimiter)
    {
        if (!delimiter.IsEnclosing()) throw new ConstraintException("Delimiter must be an Enclosing delimiter!");
        return delimiter;
    }
}