using VentLib.Options.Interfaces;
using VentLib.Utilities.Optionals;

// ReSharper disable InconsistentNaming

namespace VentLib.Options.IO;

public class IOSettings
{
    public Optional<Option> Source = Optional<Option>.Null();

    public ADEAnswer UnknownValueAction { get; set; } = ADEAnswer.UseDefault;

    public IOptionValueLoader OptionValueLoader { get; set; } = new OptionValueLoader();
}

//Allow, Default, Exception
public enum ADEAnswer
{
    Allow,
    UseDefault,
    ThrowException
}