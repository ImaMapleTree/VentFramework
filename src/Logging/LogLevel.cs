using System;
using System.Collections.Generic;
using VentLib.Options.Processors;
using VentLib.Utilities;
using Color = System.Drawing.Color;

namespace VentLib.Logging;

public struct LogLevel : IComparable<LogLevel>
{
    internal static readonly HashSet<LogLevel> Levels = new();
    internal static int LongestName = 7;

    public static LogLevel All = new("ALL", int.MinValue);
    public static LogLevel Trace = new("TRACE", -1);
    public static LogLevel Old = new("OLD", 0);
    public static LogLevel Debug = new("DEBUG", 1, Color.Goldenrod);
    public static LogLevel Info = new("INFO", 2, Color.Green);
    public static LogLevel High = new("HIGH", 3, Color.Pink);
    public static LogLevel Warn = new("WARN", 3, Color.FromArgb(255, 247, 241, 170));
    public static LogLevel Error = new("ERROR", 4, Color.Orange);
    public static LogLevel Fatal = new("FATAL", 6, ConsoleColor.DarkRed);

    public string Name;
    public Color Color;
    public int Level { get; }
    
    static LogLevel()
    {
        ValueTypeProcessors.AddTypeProcessor(new LogLevelValueTypeProcessor());
    }

    public LogLevel(string name, uint level = 0, ConsoleColor color = ConsoleColor.DarkGray)
    {
        Name = name;
        Level = (int)level;
        Color = color.ToSystemDrawing();
        Levels.Add(this);
        LongestName = Math.Max(LongestName, name.Length);
    }
    
    public LogLevel(string name, uint level = 0, Color? color = null)
    {
        Name = name;
        Level = (int)level;
        Color = color ?? Color.DarkGray;
        Levels.Add(this);
        LongestName = Math.Max(LongestName, name.Length);
    }
    
    public LogLevel(string name, uint level = 0, UnityEngine.Color? color = null)
    {
        Name = name;
        Level = (int)level;
        Color = color?.ToSystemDrawing() ?? Color.DarkGray;
        Levels.Add(this);
        LongestName = Math.Max(LongestName, name.Length);
    }

    public LogLevel(string name, uint level = 0)
    {
        
        Name = name;
        Level = (int)level;
        Color = Color.DarkGray;
        Levels.Add(this);
        LongestName = Math.Max(LongestName, name.Length);
    }

    private LogLevel(string name, int level)
    {
        Name = name;
        Level = level;
        Color = Color.DarkGray;
        Levels.Add(this);
        LongestName = Math.Max(LongestName, name.Length);
    }

    public LogLevel Similar(string name, ConsoleColor? color = null)
    {
        LogLevel level = this;
        level.Name = name;
        level.Color = color?.ToSystemDrawing() ?? Color.DarkGray;
        return level;
    }

    public int CompareTo(LogLevel other) => Level.CompareTo(other.Level);

    public override bool Equals(object? obj)
    {
        if (obj is not LogLevel otherLevel) return false;
        return otherLevel.Name == Name && otherLevel.Level == Level;
    }

    public override string ToString() => Name;

    public override int GetHashCode() => Name.GetHashCode() + Level.GetHashCode();
}