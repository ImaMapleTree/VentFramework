using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class IntProcessor : IValueTypeProcessor<int>
{
    public int Read(MonoLine input) => int.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(int value, MonoLine output) => output.Of(value.ToString());
}