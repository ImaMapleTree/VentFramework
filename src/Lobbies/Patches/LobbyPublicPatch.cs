using AmongUs.Data;
using HarmonyLib;
using VentLib.Logging;

namespace VentLib.Lobbies.Patches;

[HarmonyPatch(typeof(GameStartManager), nameof(GameStartManager.MakePublic))]
internal class LobbyPublicPatch
{
    public static void Prefix(GameStartManager __instance)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        VentLogger.Info($"Lobby Created: {AmongUsClient.Instance.GameId}", "ModdedLobbyCheck");
        LobbyChecker.POSTModdedLobby(AmongUsClient.Instance.GameId.ToString(), DataManager.Player.customization.name);
    }
}