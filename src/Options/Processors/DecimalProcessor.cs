using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class DecimalProcessor: IValueTypeProcessor<decimal>
{
    public decimal Read(MonoLine input) => decimal.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(decimal value, MonoLine output) => output.Of(value.ToString(CultureInfo.InvariantCulture));
}