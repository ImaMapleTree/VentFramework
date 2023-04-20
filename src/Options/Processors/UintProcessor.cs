using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class UintProcessor : IValueTypeProcessor<uint>
{
    public uint Read(MonoLine input) => uint.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(uint value, MonoLine output) => output.Of(value.ToString());
}