using System.Collections.Generic;
using VentLib.Options.IO;

namespace VentLib.Options.Interfaces;

public interface IOptionValueLoader
{
    public OptionValue LoadValue(List<OptionValue> choices, object input, IOSettings settings);
}