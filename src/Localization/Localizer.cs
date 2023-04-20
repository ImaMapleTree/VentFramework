using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using VentLib.Localization.Attributes;
using VentLib.Localization.Patches;
using VentLib.Logging;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;
using YamlDotNet.Serialization;

namespace VentLib.Localization;

public class Localizer
{
    internal static readonly Dictionary<Assembly, Localizer> Localizers = new();

    public Dictionary<string, Language> Languages { get; }
    public LocalizerSettings Settings { get; } = new();
    public string CurrentLanguage = LanguageSetPatch.CurrentLanguage;
    
    private Dictionary<string, string> cachedTranslations = new();
    
    private ISerializer serializer;
    private IDeserializer deserializer;

    private Localizer(Assembly assembly)
    {
        serializer = new SerializerBuilder().DisableAliases().WithNamingConvention(Settings.NamingConvention).Build();
        deserializer = new DeserializerBuilder().WithDuplicateKeyChecking()
            .WithNamingConvention(Settings.NamingConvention)
            .WithTypeMapping<Dictionary<string, object>, QualifiedDictionary>()
            .Build();

        string assemblyName = assembly == Vents.RootAssemby ? "root" : Vents.AssemblyNames.GetValueOrDefault(assembly, assembly.GetName().Name!);

        DirectoryInfo directoryInfo = assemblyName == "root" ? LocalizerSettings.LanguageDirectory : LocalizerSettings.LanguageDirectory.GetDirectory(assemblyName);
        bool createNewFile = (!directoryInfo.Exists || directoryInfo.GetFiles().Length == 0) && Settings.CreateTemplateFile;
        if (!directoryInfo.Exists) directoryInfo.Create();

        if (createNewFile)
        {
            FileInfo file = directoryInfo.GetFile("lang_Template.yaml");
            Language templateLanguage = new()
            {
                Name = "English",
                Authors = new List<string> { "Auto-Generated" },
                Translations = new QualifiedDictionary(),
                File = file
            };
            templateLanguage.Dump(serializer);
        }

        Languages = LoadLanguages(directoryInfo).ToDict(l => l.Name, l => l);
        assembly.GetTypes()
            .Where(t => t.GetCustomAttribute<LocalizedAttribute>() != null)
            .ForEach(t => t.GetCustomAttribute<LocalizedAttribute>()!.Register(this, t));
    }
    
    public static Localizer Get(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return Localizers.GetOrCompute(assembly, () => new Localizer(assembly));
    }

    public static string Translate(string qualifier, string defaultValue = "<{0}>", bool useCache = true, Assembly? assembly = null, TranslationCreationOption translationCreationOption = TranslationCreationOption.SaveIfNull)
    {
        return Get(assembly ?? Assembly.GetCallingAssembly()).Translate(qualifier, defaultValue, useCache, translationCreationOption);
    }

    public static Localizer Reload(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return Localizers[assembly] = new Localizer(assembly);
    }

    public string Translate(string qualifier, string defaultValue = "<{0}>", bool useCache = true, TranslationCreationOption translationCreationOption = TranslationCreationOption.SaveIfNull)
    {
        string? cached = cachedTranslations.GetValueOrDefault(qualifier);
        if (cached != null && useCache) return cached;
        return cachedTranslations[qualifier] = Languages[CurrentLanguage].Translate(qualifier, defaultValue, translationCreationOption);
    }

    internal string Translate(Language language, string qualifier, string defaultValue, TranslationCreationOption creationOption)
    {
        if (language.Translations.TryGet(qualifier, out object? translation)) return translation as string ?? defaultValue;
        
        if (defaultValue != null! && defaultValue.Contains("{0}")) 
            defaultValue = string.Format(defaultValue, qualifier);
        
        switch (creationOption)
        {
            case TranslationCreationOption.SaveIfNull:
                language.Translations.Set(qualifier, defaultValue, true);
                language.Dump(serializer);
                return defaultValue;
            case TranslationCreationOption.CreateIfNull:
                language.Translations.Set(qualifier, defaultValue, true);
                return defaultValue;
            case TranslationCreationOption.NothingIfNull:
                return defaultValue;
            case TranslationCreationOption.ErrorIfNull:
                throw new ArgumentException($"No Value exists for Qualifier: \"{qualifier}\"");
            default:
                throw new ArgumentOutOfRangeException(nameof(creationOption), creationOption, null);
        }
    }

    public string[] GetAllTranslations(string qualifier)
    {
        return Languages.Values.SelectWhere(t => t.Translate(qualifier, null!)).ToArray();
    }

    public Language[] FindAllLanguagesFromTranslation(string translation, string? qualifier = null)
    {
        return Languages.Select(l => l.Value).Where(lang =>
        {
            if (qualifier != null)
                return lang.Translate(qualifier, null!, TranslationCreationOption.NothingIfNull) != null!;
            return FlattenDictionaryValues(lang.Translations).Contains(translation);
        }).ToArray();
    }
    
    public Language? FindLanguageFromTranslation(string translation, string? qualifier = null) => FindAllLanguagesFromTranslation(translation, qualifier).FirstOrDefault();

    public void Register(Type type, string qualifier, bool ignoreNesting = false)
    {
        LocalizedAttribute pseudoAttribute = new(qualifier, ignoreNesting);
        pseudoAttribute.Register(this, type, new[] { pseudoAttribute });
    }
    
    private List<Language> LoadLanguages(DirectoryInfo directory)
    {
        return directory.EnumerateFiles("lang_*").SelectWhere(f =>
        {
            VentLogger.Info($"Loading Language File: {f}");
            try
            {
                StreamReader reader = new(f.Open(FileMode.OpenOrCreate), Encoding.UTF8);
                string yamlString = reader.ReadToEnd();
                reader.Close();
                Language language = deserializer.Deserialize<Language>(yamlString);
                language.File = f;
                language.Localizer = this;
                return language;
            } catch (Exception e) {
                VentLogger.Exception(e, $"Unable to load Language File \"{f.Name}\": ");
                return null;
            }
        }).ToList();
    }

    private List<string> FlattenDictionaryValues(Dictionary<string, object> dictionary)
    {
        return dictionary.Values.SelectMany(val =>
        {
            if (val is Dictionary<string, object> map) return FlattenDictionaryValues(map);
            return new List<string> { val.ToString() ?? "" };
        }).ToList();
    }
}

public enum TranslationCreationOption
{
    SaveIfNull,
    CreateIfNull,
    NothingIfNull,
    ErrorIfNull
}