using System.Collections.Generic;
using System.Drawing;

namespace VentLib.Logging.Accumulators;

public class ColorMessageAccumulator: ILogAccumulator
{
    private static Color _errorColor = Color.FromArgb(255, 176, 48, 37);
    
    public static readonly Dictionary<LogLevel, Color> LevelColors = new()
    {
        { LogLevel.Warn, LogLevel.Warn.Color },
        { LogLevel.Error, LogLevel.Error.Color },
        { LogLevel.Fatal, LogLevel.Fatal.Color }
    };

    
    public LogComposite Accumulate(LogComposite composite, LogArguments arguments)
    {
        if (arguments.Exception != null) composite.Color = _errorColor;
        else if (LevelColors.TryGetValue(composite.Level, out Color messageColor)) composite.Color = messageColor;
        return composite;
    }
}