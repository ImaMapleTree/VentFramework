using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using VentLib.Logging;
using VentLib.Logging.Default;
using VentLib.Options.Processors;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace VentLib.Options.IO;

public class EssFile
{
    private static Regex _valueRegex = new("^([^:]*):\\s?(.*)$");
    private List<string> qualifiers = new();
    private List<OrderedDictionary<EssKey, EssValue>> parents = null!;
    private string? parsingDescription;

    private OrderedDictionary<EssKey, EssValue> innerDictionary = null!;
    private Dictionary<string, (EssKey, EssValue)> flatDictionary = null!;

    public static EssFile FromPath(string path)
    {
        EssFile essFile = new();
        essFile.ParseFile(path);
        return essFile;
    }

    public OrderedDictionary<EssKey, EssValue> ParseFile(string path)
    {
        string content;
        using (StreamReader reader = new(File.Open(path, FileMode.OpenOrCreate)))
            content = reader.ReadToEnd();
        return Parse(content);
    }

    public OrderedDictionary<EssKey, EssValue> Parse(string content)
    {
        string[] lines = content.Split("\n");
        
        innerDictionary = new OrderedDictionary<EssKey, EssValue>();
        flatDictionary = new Dictionary<string, (EssKey, EssValue)>();
        parents = new List<OrderedDictionary<EssKey, EssValue>> { innerDictionary };
        lines.ForEach(l => ParseLine(l.Replace("\r", "")));
        return innerDictionary;
    }

    public void Dump(string path)
    {
        StreamWriter? writer = null;
        try
        {
            writer = new(File.Open(path, FileMode.Create));
            innerDictionary.ForEach((kvp, _) => Dump(kvp.Key, kvp.Value, writer, 0, true));
        }
        finally
        {
            writer?.Close();
        }
    }

    private void Dump(EssKey key, EssValue essValue, StreamWriter writer, int level, bool addNewLine = false)
    {
        string indent = "  ".Repeat(level - 1);
        string descriptionPrefix = indent + "# ";
        string optionPrefix = indent;
        if (level > 0) optionPrefix += "* ";
        
        key.Description?.Split("\n").ForEach(l => writer.WriteLine($"{descriptionPrefix}{l}"));

        NoDepLogger.Log(LogLevel.All, $"{optionPrefix}{key.Key}: {essValue.Value}");
        writer.WriteLine($"{optionPrefix}{key.Key}: {essValue.Value}");
        essValue.Child.ForEach(kvp => Dump(kvp.Key, kvp.Value, writer, level + 1));
        
        if (addNewLine) writer.WriteLine("");
        
    }

    internal void ApplyToOption(Option option)
    {
        string qualifier = option.Qualifier();
        if (!flatDictionary.ContainsKey(qualifier)) throw new DataException($"Configuration does not contain a value for \"{qualifier}\".");
        (EssKey _, EssValue value) = flatDictionary[qualifier];
        object objValue = ValueTypeProcessors.ReadFromLine(value.Value, option.ValueType);
        List<OptionValue> values = option.Values;
        option.IOSettings.Source = Optional<Option>.Of(option);
        OptionValue optionValue = option.IOSettings.OptionValueLoader.LoadValue(values, objValue, option.IOSettings);
        option.SetValue(optionValue);
    }

    internal void WriteToCache(Option option)
    {
        flatDictionary.GetOptional(option.Qualifier()).Handle(
                ekv => WriteToCache(option, ekv.Item1, ekv.Item2),
                () =>
                {
                    Option parent = option;
                    while (parent.HasParent()) parent = parent.Parent.Get();
                    string pQual = parent.Qualifier();
                    EssKey key;
                    EssValue value;
                    if (flatDictionary.TryGetValue(pQual, out (EssKey, EssValue) ekv))
                    {
                        key = ekv.Item1;
                        value = ekv.Item2;
                    }
                    else
                    {
                        key = new EssKey(parent.Key ?? parent.Name()) { Description = parent.Description.OrElse(null!), FullQualifier = parent.Qualifier() };
                        value = new EssValue(ValueTypeProcessors.WriteToString(parent.GetValue()));
                    }
                    innerDictionary[key] = value;
                    WriteToCache(option, key, value, false);
                }
            );
    }

    private void WriteToCache(Option option, EssKey essKey, EssValue essValue, bool doWrite = true)
    {
        if (doWrite) essValue.Value = ValueTypeProcessors.WriteToString(option.GetValue());
        flatDictionary[essKey.FullQualifier] = (essKey, essValue);
        
        
        option.Children.ForEach(child =>
        {
            string qualifier = child.Qualifier();
            if (flatDictionary.TryGetValue(qualifier, out (EssKey, EssValue) value))
            {
                WriteToCache(child, value.Item1, value.Item2);
                return;
            }

            EssKey key = new(child.Key ?? child.Name()) { Description = child.Description.OrElse(null!), FullQualifier = child.Qualifier() };
            if (essValue.Child.TryGetValue(key, out EssValue childValue))
            {
                WriteToCache(child, key, childValue);
                return;
            }

            childValue = new EssValue(ValueTypeProcessors.WriteToString(child.GetValue()));
            essValue[key] = childValue;
            WriteToCache(child, key, childValue, false);
        });
    }

    private void ParseLine(string line)
    {
        int level = StripLevelMarker(ref line) + 1;
        if (line.StartsWith("#"))
        {
            this.parsingDescription += "\n" + line.TrimStart('#');
            return;
        }

        if (line.IsNullOrWhiteSpace()) return;
        
        var groups = _valueRegex.Match(line).Groups;
        if (groups.Count != 3) throw new FormatException($"Could not parse option line: \"{line}\"");
        EssKey key = new(groups[1].Value.TrimEnd()) { Description = parsingDescription?.TrimStart().TrimEnd() };
        EssValue value = new(groups[2].Value.TrimEnd());
        parsingDescription = null;

        if (level >= parents.Count) parents.Add(value.Child);
        else parents[level] = value.Child;
        if (level > qualifiers.Count) qualifiers.Add(key.Key);
        else qualifiers[level - 1] = key.Key;

        key.FullQualifier = qualifiers.ToArray()[..level].Fuse(".");
        flatDictionary[key.FullQualifier] = (key, value);

        parents[level - 1][key] = value;
    }


    private static int StripLevelMarker(ref string line)
    {
        string formatted = line.TrimStart(' ');
        int level = (line.Length - formatted.Length) / 2;
        line = formatted.Contains('*') ? formatted[2..] : formatted;
        return level;
    }

    public class EssKey
    {
        public readonly string Key;
        public string? Description;
        public string FullQualifier = null!;

        public EssKey(string key)
        {
            this.Key = key;
        } 

        public override bool Equals(object? obj) => obj is EssKey essKey && essKey.Key.Equals(Key);

        public override int GetHashCode() => Key.GetHashCode();

        public override string ToString() => Key;
    }

    public class EssValue
    {
        public string Value;
        public OrderedDictionary<EssKey, EssValue> Child = new();

        public EssValue(string value)
        {
            Value = value;
        }
        
        public EssValue? this[EssKey key]
        {
            get => Child[key];
            set => Child[key] = value!;
        }

        public override string ToString() => Value;
    }
}