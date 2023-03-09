using HarmonyLib;

namespace VentLib.Options.Game.Patches;

[HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
internal static class StringOptionEnablePatch
{
    public static bool Prefix(StringOption __instance)
    {
        return !__instance.name.Contains("Modded");
    }
}