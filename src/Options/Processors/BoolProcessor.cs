using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class BoolProcessor : IValueTypeProcessor<bool>
{
    public bool Read(MonoLine input) => bool.Parse(input.Content);

    public MonoLine Write(bool value, MonoLine output) => output.Of(value.ToString());
}