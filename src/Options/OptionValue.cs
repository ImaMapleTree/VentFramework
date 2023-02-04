using UnityEngine;
using VentLib.Localization;
using VentLib.Utilities;

namespace VentLib.Options;

public class OptionValue
{
    public static OptionValue Null = new() { Text = Localizer.Get("NoOptionValue"), Value = new object() };
    
    public string? Text;
    public Color Color = Color.white;
    public object Value = null!;
    public string Suffix = "";

    public override string ToString()
    {
        string output = Color.Colorize((Text ?? Value.ToString() ?? Localizer.Get("NoOptionValue")) + Suffix);
        return output;
    } 

    public class OptionValueBuilder
    {
        private OptionValue value = new();

        public OptionValueBuilder Text(string text)
        {
            value.Text = text;
            return this;
        }

        public OptionValueBuilder Value(object value)
        {
            this.value.Value = value;
            return this;
        }

        public OptionValueBuilder Color(Color color)
        {
            value.Color = color;
            return this;
        }

        public OptionValueBuilder Suffix(string suffix)
        {
            value.Suffix = suffix;
            return this;
        }

        public OptionValue Build()
        {
            return value;
        }
    }
}