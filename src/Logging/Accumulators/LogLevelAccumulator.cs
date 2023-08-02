using System;

namespace VentLib.Logging.Accumulators;

public class LogLevelAccumulator: ILogAccumulator
{
    public virtual int Padding { get; } = LogLevel.LongestName;

    private bool colorizeText;
    private CharCase charCase;

    public LogLevelAccumulator(bool colorizeText = true, CharCase charCase = CharCase.AllUpper)
    {
        this.colorizeText = colorizeText;
        this.charCase = charCase;
    }
    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        LogLevel level = composite.Level;
        string levelName = charCase switch
        {
            CharCase.AllUpper => level.Name.ToUpper(),
            CharCase.FirstUpper => level.Name.Length > 1 ? char.ToUpper(level.Name[0]) + level.Name[1..] : "",
            CharCase.AllLower => level.Name.ToLower(),
            _ => throw new ArgumentOutOfRangeException()
        };

        levelName = levelName.PadRight(Padding);
        
        return composite.Append(levelName, colorizeText ? level.Color : null);
    }

    public enum CharCase
    {
        AllUpper,
        FirstUpper,
        AllLower
    }
}