using HarmonyLib;

namespace VentLib.Utilities.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.Awake))]
public class AmongUsClientPatch
{
    public static void Postfix(AmongUsClient __instance)
    {
        AUCWrapper.Instance!.RunCached();
    }
}