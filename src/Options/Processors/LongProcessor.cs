using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class LongProcessor : IValueTypeProcessor<long>
{
    public long Read(MonoLine input) => long.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(long value, MonoLine output) => output.Of(value.ToString());
}