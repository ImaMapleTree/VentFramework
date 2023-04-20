using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class UshortProcessor : IValueTypeProcessor<ushort>
{
    public ushort Read(MonoLine input) => ushort.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(ushort value, MonoLine output) => output.Of(value.ToString());
}