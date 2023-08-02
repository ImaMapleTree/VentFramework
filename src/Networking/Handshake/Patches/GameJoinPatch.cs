using System.Reflection;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Networking.RPC.Patches;
using VentLib.Version;

namespace VentLib.Networking.Handshake.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
internal static class GameJoinPatch
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(GameJoinPatch));
    public static void Prefix(AmongUsClient __instance)
    {
        HandleRpcPatch.AumUsers.Clear();
        foreach (Assembly assembly in Vents.RegisteredAssemblies.Keys)
        {
            Vents.RegisteredAssemblies[assembly] = VentControlFlag.AllowedReceiver | VentControlFlag.AllowedSender;
            Vents.BlockedReceivers[assembly] = null;
            Vents.VersionControl.PassedClients.Clear();
            Vents.VersionControl.PlayerVersions.Clear();
            Vents.VersionControl.PlayerVersions[0] = VersionControl.Instance.Version ?? new NoVersion();
        }
        log.Info("Refreshed Assembly Flags", "VentLib");
    }
}