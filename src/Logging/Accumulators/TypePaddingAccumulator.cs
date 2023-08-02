using System;
using System.Collections.Generic;
using System.Linq;
using VentLib.Utilities.Extensions;

namespace VentLib.Logging.Accumulators;

public abstract class TypePaddingAccumulator: ILogAccumulator
{
    public PaddingOption PaddingOptions;
    public int MaxLength;
    public int QualifierDepth;
    
    public TypePaddingAccumulator(int maxLength = int.MaxValue, PaddingOption paddingOption = PaddingOption.PadLeft, int qualifierDepth = int.MaxValue)
    {
        this.MaxLength = maxLength;
        this.PaddingOptions = paddingOption;
        this.QualifierDepth = qualifierDepth;
    }
    
    public abstract LogComposite Accumulate(LogComposite composite, LogArguments arguments);

    public string PadString(string str)
    {
        if (MaxLength == int.MaxValue) return str;
        return PaddingOptions is PaddingOption.PadLeft ? str.PadLeft(MaxLength) : str.PadRight(MaxLength);
    }
    
    protected string PadCaller(Type? callingType)
    {
        string fullName = callingType?.FullName ?? "Unknown";
        
        if (PaddingOptions is PaddingOption.NoPadding) return fullName;
        string[] splitQualifier = fullName.Split(".");
        splitQualifier = splitQualifier[Math.Max(splitQualifier.Length - QualifierDepth, 0)..];
        
        List<string> reverseList = new(splitQualifier.Length);
        int newStringLength = 0;
        
        for (int i = splitQualifier.Length - 1; i >= 0; i--)
        {
            string segment = splitQualifier[i];
            int rQLength = (splitQualifier.Length - (splitQualifier.Length - i)) * 2;
            int remaining = MaxLength - newStringLength - segment.Length - (rQLength > 0 ? rQLength : 0);
            
            //System.Console.Out.WriteLine($"Segment: {segment} | RQ={rQLength} | REM={remaining} | SQL={splitQualifier.Length}");
            
            if (remaining < 0)
            {
                reverseList.AddRange(splitQualifier[..(i+1)].Reverse().Select(q => q.Length > 0 ? q[0].ToString() : ""));
                break;
            }
            
            reverseList.Add(segment);
            newStringLength += segment.Length + 1;
        }

        return PadString(((IEnumerable<string>)reverseList).Reverse().Fuse("."));
    }
}