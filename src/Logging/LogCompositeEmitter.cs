using System;
using System.Drawing;

namespace VentLib.Logging;

public class LogCompositeEmitter: LogComposite
{
    public string Message => Composite.Message;
    public Color? Color { get; set; }
    public LogLevel Level => Composite.Level;

    public readonly LogComposite Composite;
    
    public LogCompositeEmitter(LogComposite composite)
    {
        this.Composite = composite;
    }

    public LogComposite Append(string text, Color? color) => Append(new CompositeSegmentValue(text, color));

    public LogComposite Append(CompositeSegment segment)
    {
        if (OnAppend == null) return Composite.Append(segment);
        
        SegmentAppendEventArgs segmentAppendEventArgs = new(segment);
        OnAppend.Invoke(segmentAppendEventArgs);
        segment = segmentAppendEventArgs.Segment;
        return Composite.Append(segment);
    }

    public LogComposite SetMessage(string message)
    {
        MessageSetEventArgs messageSetEventArgs = new(Message, message);
        OnMessageSet?.Invoke(messageSetEventArgs);
        Composite.SetMessage(messageSetEventArgs.Message);
        return this;
    }

    public event MessageSetEvent? OnMessageSet;

    public event SegmentAppendEvent? OnAppend;

    public delegate void MessageSetEvent(MessageSetEventArgs message);

    public delegate void SegmentAppendEvent(SegmentAppendEventArgs text);

    public override string ToString() => Composite.ToString()!;

    public string ToString(bool useColor) => Composite.ToString(useColor);

    public class SegmentAppendEventArgs
    {
        public CompositeSegment Segment;

        public SegmentAppendEventArgs(CompositeSegment segment)
        {
            this.Segment = segment;
        }

        public void UpdateText(Func<string, string> textUpdater)
        {
            Segment.Text = textUpdater(Segment.Text);
        }

        public void UpdateColor(Func<Color?, Color> colorUpdater)
        {
            Segment.Color = colorUpdater(Segment.GetColor());
        }
    }
    
    public class MessageSetEventArgs
    {
        public string OldMessage { get; private set; }
        public string Message { get; set; }

        public MessageSetEventArgs(string oldMessage, string message)
        {
            this.OldMessage = oldMessage;
            this.Message = message;
        }
    }
}