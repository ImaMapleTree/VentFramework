using System.Diagnostics.CodeAnalysis;
using AmongUs.Data;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Utilities.Extensions;

namespace VentLib.Localization.Patches;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.SetLanguage))]
internal class LanguageSetPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LanguageSetPatch));
    internal static string CurrentLanguage = DataManager.Settings.Language.CurrentLanguage.ToString();
    
    [SuppressMessage("ReSharper", "UnusedMember.Local")]
    private static void Postfix([HarmonyArgument(0)] TranslatedImageSet lang)
    {
        log.Info($"Loaded Language: {lang.languageID}");
        CurrentLanguage = lang.languageID.ToString();
        Localizer.Localizers.Values.ForEach(l => l.CurrentLanguage = CurrentLanguage);
    }
}