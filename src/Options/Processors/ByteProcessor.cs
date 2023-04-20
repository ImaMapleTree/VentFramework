using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class ByteProcessor : IValueTypeProcessor<byte>
{
    public byte Read(MonoLine input) => byte.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(byte value, MonoLine output) => output.Of(value.ToString());
}