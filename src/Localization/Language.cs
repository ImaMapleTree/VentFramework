using System.Collections.Generic;
using System.IO;
using System.Text;
using VentLib.Logging;
using YamlDotNet.Serialization;

namespace VentLib.Localization;

public class Language
{
    public string Name { get; set; }= null!;
    public List<string> Authors { get; set; } = null!;
    public Dictionary<object, object> Translations { get; set; } = null!;

    [YamlIgnore]
    internal FileInfo? File;

    internal void Dump()
    {
        if (File == null) return;
        VentLogger.Trace($"Saving Translation File: {File}");

        FileStream stream = File.Open(File.Exists ? FileMode.Truncate : FileMode.CreateNew);
        string yamlString = LanguageLoader.Serializer.Serialize(this);
        stream.Write(Encoding.ASCII.GetBytes(yamlString));
        stream.Close();
    }

    /// <summary>
    /// Returns a translation for the given key path
    /// </summary>
    /// <param name="keyPath">Key path of the translation</param>
    /// <returns>A translation for the given key path or "N/A" if no translation exists</returns>
    public string Translate(string keyPath) => Localizer.GetValueFromPath(this, keyPath);

    public override string ToString() => $"Language({Name})";
}