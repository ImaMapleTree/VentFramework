using VentLib.Utilities.Optionals;

namespace VentLib.Options.Interfaces;

public interface IOptionValueEvent : IOptionEvent
{
    Optional<object> OldValue();

    object NewValue();
}