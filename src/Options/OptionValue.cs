using System;
using VentLib.Utilities.Optionals;

namespace VentLib.Options;


public class OptionValue
{
    internal Optional<string> text = Optional<string>.Null();
    internal object Value { get; private set; } = null!;

    private OptionValue() {
    }
    
    internal OptionValue(object value)
    {
        Value = value ?? throw new NullReferenceException("Value in OptionValue cannot be null");
    }

    public string GetText()
    {
        return text.OrElseGet(() => Value.ToString()!);
    }

    public class OptionValueBuilder
    {
        protected Optional<string> text = Optional<string>.Null();
        protected object value = null!;

        public OptionValueBuilder Text(string text)
        {
            this.text = Optional<string>.NonNull(text);
            return this;
        }

        public OptionValueBuilder Value(object value)
        {
            this.value = value;
            return this;
        }

        public OptionValue Build()
        {
            return new OptionValue
            {
                text = text,
                Value = value
            };
        }
    }
}