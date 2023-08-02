using System;
using VentLib.Utilities.Optionals;

namespace VentLib.Options;


public class OptionValue
{
    internal Optional<string> Text = Optional<string>.Null();
    internal object Value { get; private set; } = null!;

    private OptionValue() {
    }
    
    internal OptionValue(object value)
    {
        Value = value ?? throw new NullReferenceException("Value in OptionValue cannot be null");
    }

    public string GetText()
    {
        return Text.OrElseGet(() => Value.ToString()!);
    }

    public class OptionValueBuilder
    {
        protected Optional<string> TextOptional = Optional<string>.Null();
        protected object InnerValue = null!;

        public OptionValueBuilder Text(string text)
        {
            TextOptional = Optional<string>.NonNull(text);
            return this;
        }

        public OptionValueBuilder Value(object value)
        {
            this.InnerValue = value;
            return this;
        }

        public OptionValue Build()
        {
            return new OptionValue
            {
                Text = TextOptional,
                Value = InnerValue
            };
        }
    }
}