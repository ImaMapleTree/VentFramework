using System.Linq;
using UnityEngine;
using VentLib.Utilities;

namespace VentLib.Logging.Accumulators;

public class ColoringAccumulator: IWrappingAccumulator
{
    public System.Drawing.Color Color;
    public ILogAccumulator[] WrappedAccumulators { get; }
    
    public ColoringAccumulator(Color color, params ILogAccumulator[] accumulators)
    {
        this.Color = color.ToSystemDrawing();
        this.WrappedAccumulators = accumulators;
    }
    
    public ColoringAccumulator(System.Drawing.Color color, params ILogAccumulator[] accumulators)
    {
        this.Color = color;
        this.WrappedAccumulators = accumulators;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        LogCompositeEmitter emitter = new(composite);
        emitter.OnAppend += text => text.UpdateColor(_ => Color);
        
        LogComposite aggregate = WrappedAccumulators.Aggregate((LogComposite)emitter, (cp, accumulator) => accumulator.Accumulate(cp, arguments));
        return aggregate == emitter ? emitter.Composite : aggregate;
    }
}