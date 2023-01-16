using HarmonyLib;
using InnerNet;
using VentLib.RPC;
using VentLib.Utilities;

namespace VentLib.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
internal static class PlayerJoinPatch
{
    private static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (!Vents.Settings.SendVersionCheckOnJoin) return;
        ModRPC rpc = Vents.FindRPC((uint)VentCall.VersionCheck, AccessTools.Method(typeof(VentRPC), nameof(VentRPC.SendVersionCheck)))!;
        Async.Schedule(() => rpc.Send(new[] { client.Id }, Vents.rootAssemby.GetName().FullName), 0.5f);
    }
}