using System;
using UnityEngine;
using VentLib.Utilities;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.GUI;

public class GameOptionValueBuilder
{
    private Optional<string> text = Optional<string>.Null();
    private object value = null!;    
    
    private Color color = UnityEngine.Color.white;
    private string? suffix;

    public GameOptionValueBuilder Text(string text)
    {
        this.text = Optional<string>.NonNull(text);
        return this;
    }

    public GameOptionValueBuilder Value(object value)
    {
        this.value = value;
        return this;
    }
    public GameOptionValueBuilder Color(Color color)
    {
        this.color = color;
        return this;
    }

    public GameOptionValueBuilder Suffix(string suffix)
    {
        this.suffix = suffix;
        return this;
    }
    
    public OptionValue Build()
    {
        if (value == null)
            throw new NullReferenceException("Cannot build OptionValue: wrapped value cannot be null.");
        if (suffix != null)
            text = Optional<string>.Of(text.OrElseGet(() => value.ToString()!) + suffix);
        if (color != UnityEngine.Color.white)
            text = Optional<string>.Of(color.Colorize(text.OrElseGet(() => value.ToString()!)!));
        OptionValue v = new(value)
        {
            text = text
        };
        return v;
    }
}