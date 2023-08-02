using System.Drawing;
using System.Linq;
using VentLib.Utilities;

namespace VentLib.Logging.Accumulators;

public class DelimitingAccumulator: IWrappingAccumulator
{
    public Delimiter Delimiter { get; set; }
    public ILogAccumulator[] WrappedAccumulators { get; }
    
    public DelimitingAccumulator(Delimiter delimiter, params ILogAccumulator[] accumulators)
    {
        Delimiter = delimiter;
        this.WrappedAccumulators = accumulators;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        LogCompositeEmitter emitter = new(composite);
        emitter.OnAppend += text => text.Segment = new DelimitingCompositeSegment(text.Segment, Delimiter);
        
        LogComposite aggregate = WrappedAccumulators.Aggregate((LogComposite)emitter, (cp, accumulator) => accumulator.Accumulate(cp, arguments));
        return aggregate == emitter ? emitter.Composite : aggregate;
    }

    private class DelimitingCompositeSegment : CompositeSegment
    {
        public byte Alpha => compositeSegment.Alpha;
        public byte Red => compositeSegment.Alpha;
        public byte Green => compositeSegment.Alpha;
        public byte Blue => compositeSegment.Alpha;
        public string Text { get => compositeSegment.Text; set => compositeSegment.Text = value; }
        public Color Color { set => compositeSegment.Color = value; }

        private readonly Delimiter delimiter;
        private readonly CompositeSegment compositeSegment;

        public DelimitingCompositeSegment(CompositeSegment segment, Delimiter delimiter)
        {
            compositeSegment = segment;
            this.delimiter = delimiter;
        }

        public bool GetColor(out Color c) => compositeSegment.GetColor(out c);
        
        public Color? GetColor() => compositeSegment.GetColor();

        public override string ToString() => delimiter.Enclose(compositeSegment.ToString()!);

        public string ToString(bool useColor) => delimiter.Enclose(compositeSegment.ToString(useColor));
    }
}