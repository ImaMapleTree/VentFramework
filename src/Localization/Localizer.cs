using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using HarmonyLib;
using VentLib.Localization.Attributes;
using VentLib.Localization.Patches;
using VentLib.Logging;
using VentLib.Utilities.Collections;
using VentLib.Utilities.Extensions;
using YamlDotNet.Serialization;

namespace VentLib.Localization;

public class Localizer
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(Localizer));
    internal static readonly Dictionary<Assembly, Localizer> Localizers = new();

    public Dictionary<string, Language> Languages { get; }
    public LocalizerSettings Settings { get; } = new();
    public string CurrentLanguage = LanguageSetPatch.CurrentLanguage;
    
    private Dictionary<string, string> cachedTranslations = new();
    
    internal ISerializer Serializer;
    internal IDeserializer Deserializer;

    private Localizer(Assembly assembly)
    {
        Serializer = new SerializerBuilder().DisableAliases().WithNamingConvention(Settings.NamingConvention).IncludeNonPublicProperties().Build();
        Deserializer = new DeserializerBuilder().WithDuplicateKeyChecking()
            .WithNamingConvention(Settings.NamingConvention)
            .WithTypeMapping<Dictionary<object, object>, QualifiedDictionary>()
            .IncludeNonPublicProperties()
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
            templateLanguage.Dump(Serializer);
        }

        Languages = LoadLanguages(directoryInfo).ToDict(l => l.Name, l => l);
        assembly.GetTypes()
            .Where(t => !t.IsNested)
            .Where(t => t.GetCustomAttribute<LocalizedAttribute>() != null)
            .ForEach(t => t.GetCustomAttribute<LocalizedAttribute>()!.Register(this, t));
    }
    
    public static Localizer Get(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return Localizers.GetOrCompute(assembly, () => new Localizer(assembly));
    }

    public static string Translate(string qualifier, string defaultValue = "<{}>", bool useCache = true, Assembly? assembly = null, TranslationCreationOption translationCreationOption = TranslationCreationOption.CreateIfNull)
    {
        return Get(assembly ?? Assembly.GetCallingAssembly()).Translate(qualifier, defaultValue, useCache, translationCreationOption);
    }

    public static string SmartTranslate(string qualifier, string defaultValue = "<{}>", bool useCache = false, Assembly? assembly = null, TranslationCreationOption translationCreationOption = TranslationCreationOption.NothingIfNull)
    {
        Type? callingType = AccessTools.GetOutsideCaller().DeclaringType;
        assembly ??= Assembly.GetCallingAssembly();
        if (callingType == null) return Translate(qualifier, defaultValue, useCache, assembly, translationCreationOption);
        return Get(assembly).SmartTranslate(qualifier, defaultValue, useCache, callingType, translationCreationOption);
    }

    public static Localizer Reload(Assembly? assembly = null)
    {
        assembly ??= Assembly.GetCallingAssembly();
        return Localizers[assembly] = new Localizer(assembly);
    }

    public string SmartTranslate(string qualifier, string defaultValue = "<{}>", bool useCache = false, Type? callingType = null, TranslationCreationOption translationCreationOption = TranslationCreationOption.NothingIfNull)
    { 
        callingType ??= AccessTools.GetOutsideCaller().DeclaringType;

        if (callingType == null) return Translate(qualifier, defaultValue, useCache, translationCreationOption);
        string? baseQualifier = LocalizedAttribute.ClassQualifiers.GetValueOrDefault(callingType);
        if (baseQualifier == null) return Translate(qualifier, defaultValue, useCache, translationCreationOption);
        return Translate(baseQualifier + "." + qualifier, defaultValue, useCache, translationCreationOption);
    }

    public string Translate(string qualifier, string defaultValue = "<{}>", bool useCache = true, TranslationCreationOption translationCreationOption = TranslationCreationOption.SaveIfNull)
    {
        string? cached = cachedTranslations.GetValueOrDefault(qualifier);
        if (cached != null && useCache && translationCreationOption is not TranslationCreationOption.ForceSave) return cached;
        
        
        return Languages.GetOptional(CurrentLanguage).CoalesceEmpty(() => Languages.GetOptional("English")).Transform(
            lang => cachedTranslations[qualifier] = lang.Translate(qualifier, defaultValue, translationCreationOption), 
            () => defaultValue.Replace("{}", qualifier));
    }

    internal string Translate(Language language, string qualifier, string defaultValue, TranslationCreationOption creationOption)
    {
        if (language.Translations.TryGet(qualifier, out object? translation))
        {
            string? text = translation as string;
            if (text == null && defaultValue != null! && defaultValue.Contains("{}"))
                defaultValue = defaultValue.Replace("{}", qualifier);
            if (creationOption is TranslationCreationOption.ForceSave)
            {
                language.Translations.Set(qualifier, defaultValue!, true);
                language.Updated = true;
                language.Dump(Serializer);
            }
            return text ?? defaultValue!;
        }
        
        if ( defaultValue != null! && defaultValue.Contains("{}"))
            defaultValue = defaultValue.Replace("{}", qualifier);
        
        switch (creationOption)
        {
            case TranslationCreationOption.ForceSave:
            case TranslationCreationOption.SaveIfNull:
                language.Translations.Set(qualifier, defaultValue!, true);
                language.Updated = true;
                language.Dump(Serializer);
                return defaultValue!;
            case TranslationCreationOption.CreateIfNull:
                language.Translations.Set(qualifier, defaultValue!, true);
                language.Updated = true;
                return defaultValue!;
            case TranslationCreationOption.NothingIfNull:
                return defaultValue!;
            case TranslationCreationOption.ErrorIfNull:
                throw new ArgumentException($"No Value exists for Qualifier: \"{qualifier}\"");
            default:
                throw new ArgumentOutOfRangeException(nameof(creationOption), creationOption, null);
        }
    }

    public string[] GetAllTranslations(string qualifier)
    {
        return Languages.Values.SelectWhere(t => t.Translate(qualifier, null!, TranslationCreationOption.NothingIfNull)).ToArray();
    }

    public Language[] FindAllLanguagesFromTranslation(string translation, string? qualifier = null)
    {
        return Languages.Select(l => l.Value).Where(lang =>
        {
            if (qualifier != null)
                return lang.Translate(qualifier, null!, TranslationCreationOption.NothingIfNull) == translation;
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
            log.Info($"Loading Language File: {f.Name}");
            try
            {
                StreamReader reader = new(f.Open(FileMode.OpenOrCreate), Encoding.UTF8);
                string yamlString = reader.ReadToEnd();
                reader.Close();
                Language language = Deserializer.Deserialize<Language>(yamlString);
                language.File = f;
                language.Localizer = this;
                return language;
            } catch (Exception e) {
                log.Exception($"Unable to load Language File \"{f.Name}\": ", e);
                return null;
            }
        }).ToList();
    }

    private List<string> FlattenDictionaryValues(Dictionary<object, object> dictionary)
    {
        return dictionary.Values.SelectMany(val =>
        {
            if (val is Dictionary<object, object> map) return FlattenDictionaryValues(map);
            return new List<string> { val.ToString() ?? "" };
        }).ToList();
    }
}

public enum TranslationCreationOption
{
    SaveIfNull,
    CreateIfNull,
    NothingIfNull,
    ErrorIfNull,
    ForceSave
}