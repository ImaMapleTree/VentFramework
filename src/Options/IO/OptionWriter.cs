using System.Collections.Generic;
using System.IO;
using VentLib.Logging;
using VentLib.Options.Processors;
using VentLib.Utilities.Extensions;

namespace VentLib.Options.IO;

public class OptionWriter
{
    private StreamWriter outputStream;
    
    internal OptionWriter(StreamWriter outputStream)
    {
        this.outputStream = outputStream;
    }

    public void WriteAll(List<Option> options, int level = -2, bool separateOptions = true)
    {
        options.ForEach(option =>
        {
            Write(option, level + 1);
            if (separateOptions) outputStream.WriteLine("");
        });
    }

    public void Write(Option option, int level = -1, bool addNewLine = false)
    {
        VentLogger.Log(LogLevel.All, $"Writing Option: {option.Qualifier()} | {level}");
        string indent = "  ".Repeat(level);
        string descriptionPrefix = indent + "# ";
        string optionPrefix = indent;
        if (level >= 0)
            optionPrefix += "* ";

        option.Description.IfPresent(desc =>
        {
            desc.Split("\n").ForEach(dLine => outputStream.WriteLine($"{descriptionPrefix}{dLine}"));
        });
        
        VentLogger.Log(LogLevel.All, $"{optionPrefix}{option.Key ?? option.Name()}: {ValueTypeProcessors.WriteToString(option.GetValue())}");
        outputStream.WriteLine($"{optionPrefix}{option.Key ?? option.Name()}: {ValueTypeProcessors.WriteToString(option.GetValue())}");
        WriteAll(option.Children, level, false);
        if (addNewLine) outputStream.WriteLine("");
    }

    public void Close()
    {
        outputStream.Close();
    }
}