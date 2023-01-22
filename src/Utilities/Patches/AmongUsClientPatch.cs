using HarmonyLib;

namespace VentLib.Utilities.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
internal class AmongUsClientPatch
{
    internal static void Postfix(AmongUsClient __instance)
    {
        AUCWrapper.Instance!.RunCached();
    }
}