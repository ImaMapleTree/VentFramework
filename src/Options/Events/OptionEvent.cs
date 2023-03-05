using VentLib.Options.Interfaces;

namespace VentLib.Options.Events;

public class OptionEvent : IOptionEvent
{
    private Option source;
    
    public OptionEvent(Option source)
    {
        this.source = source;
    }

    public Option Source() => source;
}