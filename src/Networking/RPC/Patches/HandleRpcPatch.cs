using HarmonyLib;
using Hazel;
using InnerNet;
using VentLib.Networking.Managers;

namespace VentLib.Networking.RPC.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public class HandleRpcPatch
{
    public static bool Prefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        return RpcManager.HandleRpc(callId, reader);
    }
}