using System.Reflection;
using HarmonyLib;
using VentLib.Logging;

namespace VentLib.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class GameJoinPatch
{
    private static void Prefix(AmongUsClient __instance)
    {
        foreach (Assembly assembly in VentFramework.RegisteredAssemblies.Keys)
        {
            VentFramework.RegisteredAssemblies[assembly] = VentControlFlag.AllowedReceiver | VentControlFlag.AllowedSender;
            VentFramework.BlockedReceivers[assembly] = null;
        }
        VentLogger.Info("Refreshed Assembly Flags", "VentFramework");
    }
}