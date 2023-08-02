using System.Collections.Generic;

namespace VentLib.Logging.Appenders;

public class InMemoryAppender: ILogAppender
{
    protected List<LogComposite> Composites = new();
    
    public void Receive(LogComposite composite, LogArguments arguments)
    {
        Composites.Add(composite);
    }

    public void Clear()
    {
        Composites.Clear();
    }
}