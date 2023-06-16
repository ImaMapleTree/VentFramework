using System;
using AmongUs.Data;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Networking;
using VentLib.Networking.Handshake.Patches;
using VentLib.Utilities;

namespace VentLib.Lobbies.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.MakePublic))]
internal class LobbyPublicPatch
{
    private static DateTime? lastUpdate;
    public static void Prefix(GameStartManager __instance)
    {
        if (lastUpdate != null && DateTime.Now.Subtract(lastUpdate.Value).TotalSeconds < 1.5f) return;
        lastUpdate = DateTime.Now;
        if (!AmongUsClient.Instance.AmHost) return;
        VentLogger.Info($"Lobby Created: {AmongUsClient.Instance.GameId}", "ModdedLobbyCheck");
        //if (!NetworkRules.AllowRoomDiscovery) return;
        VentLogger.Info("Posting Room to Public", "RoomDiscovery");
        LobbyChecker.POSTModdedLobby(AmongUsClient.Instance.GameId, DataManager.Player.customization.name);
    }

    [HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnGameJoined))]
    public static void CheckLobbyState(AmongUsClient __instance)
    {
        Async.Schedule(() =>
        {
            if (__instance == null) return;
            if (GameStartManager.Instance == null) return;
            if (__instance.IsGamePublic) Prefix(GameStartManager.Instance);
        }, 0.1f);
    }
}