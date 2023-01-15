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
        ModRPC rpc = VentFramework.FindRPC((uint)VentCall.VersionCheck, AccessTools.Method(typeof(VentRPC), nameof(VentRPC.SendVersionCheck)))!;
        rpc.Send(new[] { client.Id }, VentFramework.rootAssemby.GetName().FullName);
    }
}