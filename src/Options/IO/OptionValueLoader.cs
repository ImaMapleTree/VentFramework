using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HarmonyLib;
using VentLib.Options.Interfaces;

namespace VentLib.Options.IO;

public sealed class OptionValueLoader : IOptionValueLoader
{
    public OptionValue LoadValue(List<OptionValue> choices, object input, IOSettings settings)
    {
        OptionValue? matchingValue = choices.FirstOrDefault(ov => ov.Value.Equals(input));
        if (matchingValue != null) return matchingValue;

        Option option = settings.Source.Get();
        return settings.UnknownValueAction switch
        {
            ADEAnswer.Allow => new OptionValue(input),
            ADEAnswer.UseDefault => option.GetDefault(),
            ADEAnswer.ThrowException => throw new ConstraintException($"Cannot load illegal value \"{input}\". Choices: [{choices.Select(c => c.Value).Join()}]"),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}