using System.Reflection;
using HarmonyLib;
using VentLib.Logging;

namespace VentLib.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class GameJoinPatch
{
    private static void Prefix(AmongUsClient __instance)
    {
        foreach (Assembly assembly in Vents.RegisteredAssemblies.Keys)
        {
            Vents.RegisteredAssemblies[assembly] = VentControlFlag.AllowedReceiver | VentControlFlag.AllowedSender;
            Vents.BlockedReceivers[assembly] = null;
        }
        VentLogger.Info("Refreshed Assembly Flags", "VentLib");
    }
}