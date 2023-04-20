using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class ShortProcessor : IValueTypeProcessor<short>
{
    public short Read(MonoLine input) => short.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(short value, MonoLine output) => output.Of(value.ToString());
}