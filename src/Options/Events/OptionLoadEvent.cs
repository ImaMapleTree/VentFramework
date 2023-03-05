namespace VentLib.Options.Events;

public class OptionLoadEvent : OptionEvent
{
    public object? Value;
    
    public OptionLoadEvent(Option source, object? value) : base(source)
    {
        Value = value;
    }
}