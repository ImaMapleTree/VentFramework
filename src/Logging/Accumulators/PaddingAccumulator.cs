using System.Linq;

namespace VentLib.Logging.Accumulators;

public class PaddingAccumulator: IWrappingAccumulator
{
    public PaddingOption PaddingOption;
    public int MaxLength;
    public ILogAccumulator[] WrappedAccumulators { get; }

    public PaddingAccumulator(int maxLength, PaddingOption paddingOption, params ILogAccumulator[] accumulators)
    {
        PaddingOption = paddingOption;
        MaxLength = maxLength;
        this.WrappedAccumulators = accumulators;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        LogCompositeEmitter emitter = new(composite);
        emitter.OnAppend += text => text.UpdateText(PadString);
        
        LogComposite aggregate = WrappedAccumulators.Aggregate((LogComposite)emitter, (cp, accumulator) => accumulator.Accumulate(cp, arguments));
        return aggregate == emitter ? emitter.Composite : aggregate;
    }

    private string PadString(string str)
    {
        int nonAsciiChars = CountNonAsciiLength(str);
        if (nonAsciiChars > MaxLength) return str[..(MaxLength + (str.Length - nonAsciiChars))];
        int padding = MaxLength - nonAsciiChars;
        
        return PaddingOption is PaddingOption.PadLeft ? PadLeft(str, padding) : PadRight(str, padding);
    }

    private static string PadLeft(string str, int charCount)
    {
        if (charCount <= 0) return str;
        for (int i = 0; i < charCount; i++)
        {
            str = " " + str;
        }

        return str;
    }
    
    private static string PadRight(string str, int charCount)
    {
        if (charCount <= 0) return str;
        for (int i = 0; i < charCount; i++)
        {
            str += " ";
        }

        return str;
    }

    private static int CountNonAsciiLength(string str)
    {
        int count = 0;
        bool inAsciiColor = false;

        foreach (char c in str)
        {
            if (c is '\u001B') inAsciiColor = true;
            
            if (inAsciiColor)
            {
                if (c is 'm') inAsciiColor = false;
                continue;
            }
            
            count++;
        }

        return count;
    }
}