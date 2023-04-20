using System.Collections.Generic;
using System.IO;
using System.Text;
using VentLib.Logging;
using VentLib.Utilities.Collections;
using YamlDotNet.Serialization;

namespace VentLib.Localization;

public class Language
{
    public string Name { get; set; }= null!;
    public List<string> Authors { get; set; } = null!;
    public QualifiedDictionary Translations { get; set; } = null!;

    [YamlIgnore]
    internal FileInfo? File;

    [YamlIgnore] 
    internal Localizer Localizer;

    internal void Dump(ISerializer serializer)
    {
        if (File == null) return;
        VentLogger.Trace($"Saving Translation File: {File}");

        FileStream stream = File.Open(FileMode.Create);
        string yamlString = serializer.Serialize(this);
        stream.Write(Encoding.UTF8.GetBytes(yamlString));
        stream.Close();
    }

    /// <summary>
    /// Returns a translation for the given key path
    /// </summary>
    /// <param name="qualifier">Key path of the translation</param>
    /// <returns>A translation for the given key path or "N/A" if no translation exists</returns>
    public string Translate(string qualifier, string defaultValue = "N/A",
        TranslationCreationOption creationOption = TranslationCreationOption.SaveIfNull) =>
        Localizer.Translate(this, qualifier, defaultValue, creationOption);

    public override string ToString() => $"Language({Name})";
}