using System;
using System.Threading;
using VentLib.Utilities.Attributes;

namespace VentLib.Logging.Accumulators;

[LoadStatic]
public class ThreadAccumulator: ILogAccumulator
{
    private ThreadDisplay threadDisplay;

    public ThreadAccumulator(ThreadDisplay threadDisplay = ThreadDisplay.Name)
    {
        this.threadDisplay = threadDisplay;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        return composite.Append(threadDisplay switch
        {
            ThreadDisplay.Name => Thread.CurrentThread.Name ?? "main",
            ThreadDisplay.ID => Environment.CurrentManagedThreadId.ToString(),
            ThreadDisplay.NameAndID => Thread.CurrentThread.Name + $"({Environment.CurrentManagedThreadId})",
            _ => throw new ArgumentOutOfRangeException()
        });
    }

    public enum ThreadDisplay
    {
        Name,
        ID,
        NameAndID
    }
}