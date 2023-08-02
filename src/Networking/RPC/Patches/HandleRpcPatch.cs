using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using InnerNet;
using VentLib.Networking.Handshake;
using VentLib.Version;

namespace VentLib.Networking.RPC.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public class HandleRpcPatch
{
    internal static HashSet<string> AumUsers = new();
    private const uint VersionCheck = (uint)VentCall.VersionCheck;
    private static ModRPC _modRPC = Vents.FindRPC(VersionCheck, typeof(VersionCheck), nameof(Handshake.VersionCheck.SendVersion))!;

    public static bool Prefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        // Among Us Menu is a problem so we handle it within the library, change your call ID I dare you
        if (callId is NetworkRules.AmongUsMenuCallId)
        {
            byte playerId = reader.ReadByte();
            PlayerControl? player = PlayerControl.AllPlayerControls.ToArray().FirstOrDefault(p => p.PlayerId == playerId);
            if (player == null) return false;
            //if (AumUsers.Add(player.FriendCode)) log.SendInGame($"{player.Data.PlayerName} has joined the lobby with AmongUsMenu.");
            Vents.LastSenders[(uint)VentCall.VersionCheck] = player;
            _modRPC.InvokeTrampoline(new AmongUsMenuVersion());
            return false;
        }
        if (callId is not (203 or 204)) return true;
        RpcManager.HandleRpc(callId, reader);
        return false;
    }
}