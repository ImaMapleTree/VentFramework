using HarmonyLib;

namespace VentLib.Options.Patches;

[HarmonyPriority(Priority.First)]
[HarmonyPatch(typeof(GameOptionsMenu), nameof(GameOptionsMenu.Update))]
internal static class OptionUpdatePatch
{
    internal static void Postfix(GameOptionsMenu __instance)
    {
        OptionManager.NewRegisters.Do(o => o.RenderInit());
        OptionManager.NewRegisters.Clear();
    }
}