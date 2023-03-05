using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Options.Processors;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.IO;

public class OptionReader
{
    private static Regex _valueRegex = new("^([^:]*):\\s?(.*)$");
    private StreamReader reader;
    private readonly Dictionary<string, MonoLine> qualifiedLines = new();
    
    internal OptionReader(StreamReader reader)
    {
        this.reader = reader;
    }

    public void ReadToEnd()
    {
        List<string> qualifier = new List<string> { "" };
        int lastLevel = 0;
        while (!reader.EndOfStream)
        {
            string? line = reader.ReadLine();
            if (string.IsNullOrEmpty(line)) continue;
            int level = CalculateLevelAndFormat(ref line);
            if (line.StartsWith("#")) continue;

            var groups = _valueRegex.Match(line).Groups;
            if (groups.Count != 3) throw new FormatException($"Could not parse option line: \"{line}\"");
            string name = groups[1].Value;
            string value = groups[2].Value;
            
            if (level > lastLevel) qualifier.Add(name);
            else {
                for (int i = 0; i < lastLevel - level; i++) qualifier.PopLast();
                qualifier[level] = name;
            }

            lastLevel = level;
            qualifiedLines.Add(qualifier.Join(delimiter: "."), new MonoLine(value));
        }
        reader.Close();
    }

    public void Update(Option source)
    {
        string qualifier = source.Qualifier();
        if (!qualifiedLines.ContainsKey(qualifier))
            throw new ArgumentNullException($"Could not find stored data for \"{qualifier}\"");
        MonoLine line = qualifiedLines[qualifier];
        object objValue = ValueTypeProcessors.ReadFromLine(line, source.ValueType);
        List<OptionValue> values = source.Values;
        source.IOSettings.Source = Optional<Option>.Of(source);
        OptionValue value = source.IOSettings.OptionValueLoader.LoadValue(values, objValue, source.IOSettings);
        source.SetValue(value);
    }

    private int CalculateLevelAndFormat(ref string line)
    {
        string formatted = line.TrimStart(' ');
        int level = (line.Length - formatted.Length) / 2;
        line = formatted.Contains('*') ? formatted[2..] : formatted;
        return level;
    }
}