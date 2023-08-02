using InnerNet;
using VentLib.Logging;
using VentLib.Networking;
using VentLib.Utilities.Harmony.Attributes;

namespace VentLib.Lobbies.Patches;

internal class LobbyStatusPatches
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LobbyStatusPatches));
    [QuickPrefix(typeof(AmongUsClient), nameof(AmongUsClient.StartGame))]
    private static void UpdateStatusInGame(AmongUsClient __instance)
    {
        if (!__instance.AmHost) return;
        if (!NetworkRules.AllowRoomDiscovery) return;
        log.Info($"Updating Lobby Status: {LobbyStatus.InGame}", "LobbyStatus");
        LobbyChecker.UpdateModdedLobby(__instance.GameId, LobbyStatus.InGame);
    }
    
    [QuickPrefix(typeof(InnerNetClient), nameof(InnerNetClient.DisconnectInternal))]
    private static void UpdateStatusClosed(InnerNetClient __instance, DisconnectReasons reason)
    {
        if (reason is DisconnectReasons.NewConnection || !__instance.AmHost) return;
        if (!NetworkRules.AllowRoomDiscovery) return;
        log.Info($"Updating Lobby Status: {LobbyStatus.Closed}", "LobbyStatus");
        LobbyChecker.UpdateModdedLobby(__instance.GameId, LobbyStatus.Closed);
    }
    
    [QuickPostfix(typeof(LobbyBehaviour), nameof(LobbyBehaviour.Start))]
    private static void UpdateStatusStart(LobbyBehaviour __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        if (!NetworkRules.AllowRoomDiscovery) return;
        log.Info($"Updating Lobby Status: {LobbyStatus.Open}", "LobbyStatus");
        LobbyChecker.UpdateModdedLobby(AmongUsClient.Instance.GameId, LobbyStatus.Open);
    }
}