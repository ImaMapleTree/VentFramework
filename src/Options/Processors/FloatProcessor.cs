using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class FloatProcessor : IValueTypeProcessor<float>
{
    public float Read(MonoLine input) => float.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(float value, MonoLine output) => output.Of(value.ToString(CultureInfo.InvariantCulture));
}