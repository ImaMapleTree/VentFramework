using HarmonyLib;
using Hazel;
using InnerNet;

namespace VentLib.Networking.RPC.Patches;

[HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
public class HandleRpcPatch
{
    public static bool Prefix(InnerNetObject __instance, [HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
    {
        if (callId is not (203 or 204)) return true;
        RpcManager.HandleRpc(callId, reader);
        return false;
    }
}