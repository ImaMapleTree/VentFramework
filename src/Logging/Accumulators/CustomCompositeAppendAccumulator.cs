using static VentLib.Logging.LogCompositeEmitter;

namespace VentLib.Logging.Accumulators;

public class CustomCompositeAppendAccumulator: ILogAccumulator
{
    private readonly SegmentAppendEvent segmentAppendEventHandler;
    
    public CustomCompositeAppendAccumulator(SegmentAppendEvent segmentAppendEventHandler)
    {
        this.segmentAppendEventHandler = segmentAppendEventHandler;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        LogCompositeEmitter emitter = new(composite);
        emitter.OnAppend += segmentAppendEventHandler;
        return emitter;
    }
}