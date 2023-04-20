using System.Globalization;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;

namespace VentLib.Options.Processors;

internal class DoubleProcessor : IValueTypeProcessor<double>
{
    public double Read(MonoLine input) => double.Parse(input.Content, CultureInfo.InvariantCulture);

    public MonoLine Write(double value, MonoLine output) => output.Of(value.ToString(CultureInfo.InvariantCulture));
}