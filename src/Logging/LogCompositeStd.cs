using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Pastel;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Logging;

internal sealed class LogCompositeStd: LogComposite
{
    private readonly List<CompositeSegment> segments = new();
    public string Message { get; private set; }
    public Color? Color { get; set; }
    public LogLevel Level { get; }
    private Delimiter delimiter;

    public LogCompositeStd(LogLevel level, string message, Delimiter? delimiter = null)
    {
        Level = level;
        Message = message;
        this.delimiter = delimiter ?? Delimiter.Space;
    }

    public LogComposite Append(string text, Color? color)
    {
        return Append(new CompositeSegmentValue(text, color));
    }

    public LogComposite Append(CompositeSegment segmentValue)
    {
        segments.Add(segmentValue);
        return this;
    }

    public LogComposite SetMessage(string message)
    {
        Message = message;
        return this;
    }

    private string ColoredMessage => Color != null ? Message.Pastel(Color.Value) : Message;
    
    public override string ToString() => segments.Select(s => s.ToString(true)).Fuse(delimiter.Value()) + delimiter.Value() + ColoredMessage;
    
    public string ToString(bool useColor) => segments.Select(s => s.ToString(useColor)).Fuse(delimiter.Value()) + delimiter.Value() + (useColor ? ColoredMessage : Message);
}