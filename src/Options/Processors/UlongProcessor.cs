using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class UlongProcessor : IValueTypeProcessor<ulong>
{
    public ulong Read(MonoLine input) => ulong.Parse(input.Content);

    public MonoLine Write(ulong value, MonoLine output) => output.Of(value.ToString());
}