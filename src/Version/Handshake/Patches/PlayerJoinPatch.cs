using System.Collections.Generic;
using HarmonyLib;
using InnerNet;
using VentLib.RPC;
using VentLib.Utilities;

namespace VentLib.Version.Handshake.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
internal static class PlayerJoinPatch
{
    internal static HashSet<int> WaitSet = new();

    internal static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        VersionControl vc = VersionControl.Instance;
        if (!vc.Handshake) return;
        uint versionCheck = (uint)VentCall.VersionCheck;
        ModRPC rpc = Vents.FindRPC(versionCheck, AccessTools.Method(typeof(VersionCheck), nameof(VersionCheck.RequestVersion)))!;
        WaitSet.Add(client.Id);
        Async.Schedule(() =>
        {
            rpc.Send(new[] { client.Id });
            if (vc.ResponseTimer <= 0) return;
            Async.Schedule(() =>
            {
                if (!WaitSet.Contains(client.Id)) return;
                Vents.LastSenders[versionCheck] = client.Character;
                Vents.FindRPC(versionCheck, AccessTools.Method(typeof(VersionCheck), nameof(VersionCheck.SendVersion))!)!
                    .InvokeTrampoline(new object[] { new NoVersion() });
            }, vc.ResponseTimer);
        }, 0.5f);
    }
}