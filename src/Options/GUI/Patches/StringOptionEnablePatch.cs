using HarmonyLib;

namespace VentLib.Options.GUI.Patches;

[HarmonyPatch(typeof(StringOption), nameof(StringOption.OnEnable))]
public static class StringOptionEnablePatch
{
    public static bool Prefix(StringOption __instance)
    {
        return !__instance.name.Contains("Modded");
    }
}