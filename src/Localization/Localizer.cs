using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using AmongUs.Data;
using HarmonyLib;
using VentLib.Localization.Attributes;
using VentLib.Logging;

namespace VentLib.Localization;

public static class Localizer
{
    /// <summary>
    /// Get or set the folder where languages for the Localizer exist
    /// </summary>
    public static string LanguageFolder = "./Languages/";
    /// <summary>
    /// Get or set the Default Language
    /// </summary>
    public static string DefaultLanguage = "English";
    internal static string CurrentLanguage
    {
        get => _currentLanguage;
        set {
            bool different = _currentLanguage != value;
            _currentLanguage = value;
            if (!different) return;
            _translations = _loader.Get(_currentLanguage);
            _translationCache.Clear();
        }
    }

    private static Dictionary<string, Language> _translations = null!;
    private static Dictionary<(string, string), string> _translationCache = new();

    private static string _currentLanguage = DefaultLanguage;
    private static LanguageLoader _loader = null!;
    private static string? _root => Vents.AssemblyNames.GetValueOrDefault(Vents.RootAssemby);

    /// <summary>
    /// Provided a valid key-path, returns a translation for the current client's language. These translations are created
    /// per assembly, so it may be necessary to provide a valid assembly name.
    /// <br/>
    /// This method provides translations based on the calling assembly. Sometimes this can result in undefined behaviour.
    /// If issues occur you can provide the Simple Name of an assembly to get translations for that assembly.
    /// <br/>
    /// <b>Example of a valid key-path:</b>
    /// <code>Messages.Announcements.System</code>
    /// </summary>
    /// <param name="keyPath">The key / keyPath to provide a translation</param>
    /// <param name="assemblyName">Optional assembly name to get translations</param>
    /// <param name="useCache">If the returned translation should be cached / accessed from cache</param>
    /// <returns>The translated string or "N/A" if no translations are provided</returns>
    public static string Get(string keyPath, string? assemblyName = null, bool useCache = true)
    {
        assemblyName ??= Assembly.GetCallingAssembly().GetName().Name!;
        assemblyName = _root == assemblyName ? "root" : assemblyName;

        var cacheKey = (keyPath, assemblyName);

        string? translation;
        if (useCache) {
            translation = _translationCache.GetValueOrDefault(cacheKey);
            if (translation != null) return translation;
        }

        if (_translations.TryGetValue(assemblyName, out Language? language))
            return GetValueFromPath(language, keyPath);
        VentLogger.Fatal($"No Translations Exist for \"{assemblyName}\" Attempting to use Root ({_root}) Translations");
        language = _translations["root"];

        translation = GetValueFromPath(language, keyPath);
        if (useCache) _translationCache[cacheKey] = translation;
        return translation;
    }

    /// <summary>
    /// Returns all languages containing the provided translation for the given key-path.
    /// </summary>
    /// <param name="keyPath">keyPath to check the translation</param>
    /// <param name="translation">The translation to search for</param>
    /// <returns>List of languages containing the specified translation for the given key path</returns>
    public static List<Language> GetLanguages(string keyPath, string translation)
    {
        string assemblyName = Assembly.GetCallingAssembly().GetName().Name!;
        assemblyName = _root == assemblyName ? "root" : assemblyName;

        return  _loader.SupportedLanguages.GetValueOrDefault(assemblyName, new HashSet<string>())
            .Select(sl => _loader.Get(sl).GetValueOrDefault(assemblyName))
            .Where(lang => lang != null)
            .Select(lang => lang!)
            .Where(lang => lang.Translate(keyPath) == translation)
            .ToList();
    }

    /// <summary>
    /// Gets ALL translations (from all localizations) given a specific key-path. Similar to Localizer.Get(), this function utilizes Assembly.<see cref="Assembly.GetCallingAssembly()"/>
    /// to provide context for which assembly to get translations for. If this causes issues then you can provide a valid Simple Assembly Name to specific
    /// which assembly to grab translations from.
    /// </summary>
    /// <param name="keyPath">key path of the translation</param>
    /// <param name="assemblyName">Name of the assembly to get translations from</param>
    /// <returns>List of all translations, from all languages, for a given key path</returns>
    public static List<string> GetAll(string keyPath, string? assemblyName = null)
    {
        assemblyName ??= Assembly.GetCallingAssembly().GetName().Name!;
        assemblyName = _root == assemblyName ? "root" : assemblyName;

        List<Language?>? languages = _loader.SupportedLanguages.GetValueOrDefault(assemblyName)?
            .Select(sl => _loader.Get(sl).GetValueOrDefault(assemblyName))
            .Where(lang => lang != null).ToList();

        return languages == null ? new List<string>() : languages.Select(lang => GetValueFromPath(lang!, keyPath)).ToList();
    }

    /// <summary>
    /// Initializes all translations
    /// </summary>
    public static void Initialize()
    {
        _currentLanguage = DataManager.Settings.Language.CurrentLanguage.ToString();
        _loader = LanguageLoader.Load(LanguageFolder);
        _translations = _loader.Get(CurrentLanguage);
    }

    internal static void Load(Assembly assembly)
    {
        string assemblyName = assembly == Vents.RootAssemby ? "root" : assembly.GetName().Name!;
        VentLogger.Info($"Loading Translations for {assemblyName}");
        if (!_loader.SupportedLanguages.ContainsKey(assemblyName) && assemblyName != "root")
            new DirectoryInfo(LanguageFolder).CreateSubdirectory(assemblyName);

        if (!_translations.TryGetValue(assemblyName, out Language? language))
        {
            VentLogger.Fatal($"No Translations Exist for {assembly.GetName().Name}! Attempting to use Root Translations");
            language = _translations["root"];
        }

        assembly.GetTypes().Do(cls => ReflectionLoader.RegisterClass(cls));
        Inject(language);
    }

    private static void Inject(Language language)
    {
        List<LocalizedAttribute> sortedAttributes = LocalizedAttribute.Attributes.Keys.ToList();
        sortedAttributes.Sort();

        foreach (LocalizedAttribute attribute in sortedAttributes)
        {
            ReflectionObject reflectionObject = LocalizedAttribute.Attributes[attribute];
            if (attribute.Source?.ReflectionType is ReflectionType.Class) continue;
            string value = GetValueFromPath(language, attribute.GetPath(), defaultValue: attribute.Source?.GetValue()?.ToString());
            reflectionObject.SetValue(value);
        }

        language.Dump();
    }

    internal static string GetValueFromPath(Language language, string path, bool createIfNull = true, string? defaultValue = null)
    {
        Dictionary<object, object> dictionary = language.Translations;
        bool created = false;

        string[] subPaths = path.Split(".");
        for (int i = 0; i < subPaths.Length - 1; i++)
        {
            string subPath = subPaths[i];
            if (createIfNull && !dictionary.ContainsKey(subPath))
            {
                dictionary[subPath] = new Dictionary<object, object>();
                created = true;
            }
            dictionary = (Dictionary<object, object>)dictionary[subPath];
        }

        string finalPath = subPaths[^1];

        if (createIfNull && !dictionary.ContainsKey(finalPath)) {
            created = true;
            dictionary[finalPath] = defaultValue ?? "N/A";
        }

        if (created) language.Dump();

        return (string)dictionary[finalPath];
    }
}