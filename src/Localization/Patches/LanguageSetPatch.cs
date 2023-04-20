using AmongUs.Data;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Utilities.Extensions;

namespace VentLib.Localization.Patches;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.SetLanguage))]
internal class LanguageSetPatch
{
    internal static string CurrentLanguage = DataManager.Settings.Language.CurrentLanguage.ToString();
    
    private static void Postfix(TranslationController __instance, [HarmonyArgument(0)] TranslatedImageSet lang)
    {
        VentLogger.Info($"Loaded Language: {lang.languageID}");
        CurrentLanguage = lang.languageID.ToString();
        Localizer.Localizers.Values.ForEach(l => l.CurrentLanguage = CurrentLanguage);
    }
}