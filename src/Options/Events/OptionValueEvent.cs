using VentLib.Options.Interfaces;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.Events;

public class OptionValueEvent : OptionEvent, IOptionValueEvent
{
    private Optional<object> oldValue;
    private object newValue;

    public OptionValueEvent(Option source, Optional<object> oldValue, object newValue) : base(source)
    {
        this.oldValue = oldValue;
        this.newValue = newValue;
    }

    public Optional<object> OldValue() => oldValue;

    public object NewValue() => newValue;
}