namespace VentLib.Options.Events;

public class OptionRegisterEvent : OptionEvent
{
    public readonly OptionManager Manager;
    
    public OptionRegisterEvent(Option source, OptionManager manager) : base(source)
    {
        Manager = manager;
    }
}