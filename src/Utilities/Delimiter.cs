using System.Collections.Generic;
using System.Data;
using VentLib.Utilities.Extensions;

namespace VentLib.Utilities;

public class Delimiter
{
    public static readonly Delimiter Brackets = new("[", "]");
    public static readonly Delimiter BracketsPadded = new("[ ", " ]");
    public static readonly Delimiter Braces = new("{", "}");
    public static readonly Delimiter Parenthesis = new("(", ")");
    public static readonly Delimiter Comma = new(",");
    public static readonly Delimiter Space = new(" ");
    public static readonly Delimiter None = new("", "");
    
    private readonly string[] delimiters;
    
    public Delimiter(string delimiter)
    {
        delimiters = new[] { delimiter };
    }
    
    public Delimiter(string leftDelimiter, string rightDelimiter)
    {
        delimiters = new[] { leftDelimiter, rightDelimiter };
    }

    public bool IsSeparator() => delimiters.Length == 1 || this == None;

    public bool IsEnclosing() => delimiters.Length >= 2;

    public string Enclose(string? input)
    {
        if (!IsEnclosing()) 
            throw new ConstraintException($"Cannot enclose a string with a non-enclosing delimiter. ({GetDelimiter()})");
        return $"{delimiters[0]}{input}{delimiters[1]}";
    }

    public string Separate(IEnumerable<string> enumerable)
    {
        if (!IsSeparator())
            throw new ConstraintException($"Cannot separate an enumerable with a non-separator delimiter. ({GetDelimiter()})");
        return enumerable.Fuse(delimiters[0]);
    }

    public string Value(int index = 0) => delimiters[index];

    private string GetDelimiter() => delimiters.Fuse();
}