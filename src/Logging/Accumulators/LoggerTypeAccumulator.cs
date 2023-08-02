using System;
using VentLib.Utilities.Collections;

namespace VentLib.Logging.Accumulators;

public class LoggerTypeAccumulator : TypePaddingAccumulator
{
    public readonly DefaultDictionary<Type, string> FormattedTypes;

    public LoggerTypeAccumulator(int maxLength = int.MaxValue, PaddingOption paddingOption = PaddingOption.PadLeft, int qualifierDepth = int.MaxValue) : base(maxLength, paddingOption, qualifierDepth)
    {
        FormattedTypes = new DefaultDictionary<Type, string>(PadCaller);
    }
    
    public override LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        return composite.Append(FormattedTypes.Get(arguments.Logger.DeclaringType ?? typeof(void)));
    }
}