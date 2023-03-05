using System.Data;
using System.Text.RegularExpressions;

namespace VentLib.Options.IO;


/// <summary>
/// Wrapper class around a <see cref="string"/> to explicitly describe its content as being on a singular line.
/// </summary>
public class MonoLine
{
    private static readonly Regex NewlineRegex = new("\n|\r");
    
    internal MonoLine(string content = "")
    {
        Content = content;
    }
    
    public string Content { get; private set; }

    public MonoLine Of(string content)
    {
        Content = content;

        if (NewlineRegex.IsMatch(content))
            throw new ConstraintException("Content cannot contain new-line character.");
        
        return this;
    }

    public override string ToString() => Content;
}