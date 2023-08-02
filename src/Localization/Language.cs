using System.Collections.Generic;
using System.IO;
using System.Text;
using VentLib.Logging;
using VentLib.Utilities.Collections;
using YamlDotNet.Serialization;

namespace VentLib.Localization;

public class Language
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Language));
    public string Name { get; set; }= null!;
    public List<string> Authors { get; set; } = null!;
    internal QualifiedDictionary Translations { get; set; } = null!;

    [YamlIgnore]
    internal bool Updated;
    
    [YamlIgnore]
    internal FileInfo? File;

    [YamlIgnore] 
    internal Localizer? Localizer;

    internal void Dump(ISerializer serializer)
    {
        if (File == null) return;
        log.Trace($"Saving Translation File: {File.Name}");

        FileStream stream = File.Open(FileMode.Create);
        string yamlString = serializer.Serialize(this);
        stream.Write(Encoding.UTF8.GetBytes(yamlString));
        stream.Close();
        Updated = false;
    }

    /// <summary>
    /// Returns a translation for the given key path
    /// </summary>
    /// <param name="qualifier">Key path of the translation</param>
    /// <param name="defaultValue">The default value for the translation (if none exists)</param>
    /// <param name="creationOption">The action to be taken if the translation doesn't exist</param>
    /// <returns>A translation for the given key path or the default if no translation exists</returns>
    public string Translate(
        string qualifier, 
        string defaultValue = "<{}>",
        TranslationCreationOption creationOption = TranslationCreationOption.SaveIfNull
    )
    {
        return Localizer?.Translate(this, qualifier, defaultValue, creationOption) ?? defaultValue;
    }

    public override string ToString() => $"Language({Name})";
}