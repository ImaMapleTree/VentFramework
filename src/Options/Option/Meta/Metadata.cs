using System.Collections.Generic;
using System.IO;
using System.Linq;
using VentLib.Utilities.Extensions;
using static System.String;

namespace VentLib.Options.Meta;

internal class Metadata
{
    private readonly Dictionary<string, Option.OptionStub> qualifiers = new();
    private FileInfo optionFile;

    private Metadata(FileInfo optionFile)
    {
        this.optionFile = optionFile;
    }
    
    internal Option.OptionStub? GetStub(string qualifier) => qualifiers.GetValueOrDefault(qualifier);
    
    internal void AddStub(Option.OptionStub stub) => qualifiers[stub.Qualifier()] = stub;
    
    internal static Metadata Parse(FileInfo file)
    {
        Metadata metadata = new Metadata(file);
        Option.OptionStub? parentStub = null;
        string metaText = file.ReadAll(true);
        Queue<string> lines = new Queue<string>(metaText.Split("\n"));

        int lastLevel = 0;
        while (lines.Count > 0)
        {
            string line = lines.Dequeue();
            if (line == "") continue;
            int thisLevel = DetermineLevel(ref line);
            Option.OptionStub stub = new Option.OptionStub(line, thisLevel);
            
            if (parentStub == null) parentStub = stub;
            else if (thisLevel > lastLevel) parentStub = stub;
            else parentStub = parentStub.Parent;
            
            metadata.AddStub(stub);
            lastLevel = thisLevel;
        }

        return metadata;
    }

    public void DumpAll(List<Option> options)
    {
        string optionOutput = DumpOptions(options);
        File.WriteAllText(optionFile.FullName, optionOutput);
    }
    
    private string DumpOptions(List<Option> optionList) => Join("\n\n", optionList.Select(DumpOption));

    private string DumpOption(Option option)
    {
        string optionString = new Option.OptionStub(option).ToString();
        optionString += "\n" + Join((string?)"\n", (IEnumerable<string?>)option.SubOptions.Select(DumpOption));
        return optionString.TrimEnd('\n');
    }

    private static int DetermineLevel(ref string line)
    {
        int level = 0;
        while (line.StartsWith(">>") || line.StartsWith("  "))
        {
            level++;
            line = line.ReplaceFirst(line.StartsWith(">>") ? ">>" : "  ", "");
        }

        return level;
    }
}