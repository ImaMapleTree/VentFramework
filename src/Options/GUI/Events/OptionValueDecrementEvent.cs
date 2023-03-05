using VentLib.Options.Events;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.GUI.Events;

public class OptionValueDecrementEvent : OptionValueEvent
{
    public OptionValueDecrementEvent(Option source, Optional<object> oldValue, object newValue) : base(source, oldValue, newValue)
    {
    }
}