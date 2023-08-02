namespace VentLib.Logging;

// ReSharper disable once InconsistentNaming
public interface LogComposite
{
    public string Message { get; }
    public System.Drawing.Color? Color { get; set; }
    public LogLevel Level { get; }

    public LogComposite Append(string text, System.Drawing.Color? color = null);

    public LogComposite Append(CompositeSegment segment);

    public LogComposite SetMessage(string message);

    public string ToString(bool useColor);
}