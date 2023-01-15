using HarmonyLib;
using InnerNet;
using VentLib.RPC;

namespace VentLib.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
internal static class PlayerJoinPatch
{
    private static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (!VentFramework.Settings.SendVersionCheckOnJoin) return;
        ModRPC rpc = VentFramework.FindRPC((uint)VentRPC.VersionCheck, AccessTools.Method(typeof(VentRPCs), nameof(VentRPCs.SendVersionCheck)))!;
        rpc.Send(new[] { client.Id }, VentFramework.rootAssemby.GetName().FullName);
    }
}