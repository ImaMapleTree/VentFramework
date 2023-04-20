using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class SbyteProcessor : IValueTypeProcessor<sbyte>
{
    public sbyte Read(MonoLine input) => sbyte.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(sbyte value, MonoLine output) => output.Of(value.ToString());
}