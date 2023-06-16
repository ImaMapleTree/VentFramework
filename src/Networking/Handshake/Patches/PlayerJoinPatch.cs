using System.Collections.Generic;
using HarmonyLib;
using InnerNet;
using VentLib.Networking.RPC;
using VentLib.Utilities;
using VentLib.Version;

namespace VentLib.Networking.Handshake.Patches;

[HarmonyPatch(typeof(AmongUsClient), nameof(AmongUsClient.OnPlayerJoined))]
internal static class PlayerJoinPatch
{
    private const uint VersionCheck = (uint)VentCall.VersionCheck;
    internal static readonly HashSet<int> WaitSet = new();
    private static ModRPC _modRPC = Vents.FindRPC(VersionCheck, typeof(VersionCheck), nameof(Handshake.VersionCheck.SendVersion))!;

    internal static void Postfix(AmongUsClient __instance, [HarmonyArgument(0)] ClientData client)
    {
        if (!AmongUsClient.Instance.AmHost) return;
        VersionControl vc = VersionControl.Instance;
        if (!vc.Handshake) return;
        
        ModRPC rpc = Vents.FindRPC(VersionCheck, AccessTools.Method(typeof(VersionCheck), nameof(Handshake.VersionCheck.RequestVersion)))!;
        WaitSet.Add(client.Id);
        Async.Schedule(() =>
        {
            rpc.Send(new[] { client.Id });
            if (vc.ResponseTimer <= 0) return;
            Async.Schedule(() =>
            {
                if (!WaitSet.Contains(client.Id)) return;
                Async.WaitUntil(() => client.Character, player =>
                {
                    if (!WaitSet.Contains(client.Id)) return;
                    Vents.LastSenders[VersionCheck] = player;
                    _modRPC.InvokeTrampoline(new NoVersion());
                }, 0.15f, 40, true);
            }, vc.ResponseTimer);
        }, NetUtils.DeriveDelay(1.5f));
    }
}