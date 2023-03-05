using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class StringProcessor : IValueTypeProcessor<string>
{
    public string Read(MonoLine input) => input.Content;

    public MonoLine Write(string value, MonoLine output) => output.Of(value);
}