using HarmonyLib;
using VentLib.Logging;

namespace VentLib.Localization.Patches;

[HarmonyPatch(typeof(TranslationController), nameof(TranslationController.SetLanguage))]
internal class LanguageSetPatch
{
    private static void Postfix(TranslationController __instance, [HarmonyArgument(0)] TranslatedImageSet lang)
    {
        VentLogger.Info($"Loaded Language: {lang.languageID}");
        Localizer.CurrentLanguage = lang.languageID.ToString();
    }
}