using System.IO;
using System.Reflection;
using VentLib.Options;
using VentLib.Options.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace VentLib.Localization;

public class LocalizerSettings
{
    private static string _languageFolder;
    public static DirectoryInfo LanguageDirectory { get; }
    
    static LocalizerSettings()
    {
        OptionManager manager = new(Assembly.GetExecutingAssembly(), "locale.config");
        Option languageFolderOption = new OptionBuilder().Name("Language Folder")
            .Description("Folder where translations are stored")
            .Value("Languages")
            .IOSettings(settings => settings.UnknownValueAction = ADEAnswer.Allow)
            .BuildAndRegister(manager);

        _languageFolder = languageFolderOption.GetValue<string>();
        LanguageDirectory = new DirectoryInfo(_languageFolder);
        if (!LanguageDirectory.Exists) LanguageDirectory.Create();
    }

    public INamingConvention NamingConvention = PascalCaseNamingConvention.Instance;

    public string? ForceLanguage = null;

    public bool CreateTemplateFile = true;
}