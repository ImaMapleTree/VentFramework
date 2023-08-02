using System;
using System.Linq;
using System.Threading;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Networking.Batches;
using VentLib.Networking.Helpers;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Attributes;
using VentLib.Networking.RPC.Interfaces;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Networking.RPC;

internal class DetouredSender
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(DetouredSender));
    private readonly int uuid = UnityEngine.Random.RandomRangeInt(0, 999999);
    private int localSendCount;
    private readonly ModRPC modRPC;
    private readonly uint callId;
    private readonly RpcActors senders;
    private readonly RpcActors receivers;
    private bool batchSend;

    internal DetouredSender(ModRPC modRPC)
    {
        this.modRPC = modRPC;
        callId = this.modRPC.CallId;
        senders = modRPC.Senders;
        receivers = this.modRPC.Receivers;
        modRPC.Sender = this;
        batchSend = this.modRPC.Parameters.Any(p => p.IsAssignableTo(typeof(IBatchSendable)));
    }

    internal void IntermediateSend(params object?[] args)
    {
        if (modRPC.Invocation is MethodInvocation.ExecuteBefore) modRPC.InvokeTrampoline(args!);
        Send(null, args);
        if (modRPC.Invocation is MethodInvocation.ExecuteAfter) modRPC.InvokeTrampoline(args!);
    }

    private void DelayedSend(int[]? targets, object?[] args)
    {
        Async.WaitUntil(() => AmongUsClient.Instance != null && PlayerControl.LocalPlayer != null, b => b, _ => Send(targets, args), 0.5f, 10, true);
    }

    internal void Send(int[]? targets, object?[] args)
    {
        if (AmongUsClient.Instance == null || PlayerControl.LocalPlayer == null) {
            DelayedSend(targets, args);
            return;
        }
        if (!CanSend(out int[]? lastSender) || !Vents.CallingAssemblyFlag().HasFlag(VentControlFlag.AllowedSender)) return;
        lastSender ??= targets;
        if (batchSend) BatchSend(lastSender, args);
        else RealSend(lastSender, args);
    }

    private void RealSend(int[]? targets, object?[] args)
    {
        RpcHookHelper.GlobalSendCount++; localSendCount++;
        MonoRpc monoRpc = RpcV3.Immediate(PlayerControl.LocalPlayer.NetId, 203).Protected(false).ThreadSafe(true);
        RpcBody rpcBody = RpcBody.Of(callId).Write((byte)receivers).WritePacked(PlayerControl.LocalPlayer.NetId);
        args.Do(a => rpcBody.Write(a!));
        FinalizeV2(targets, monoRpc.SetBody(rpcBody));
    }

    private void BatchSend(int[]? targets, object?[] args)
    {
        localSendCount++;
        uint batchId = BatchWriter.BatchId++;
        MonoRpc monoRpc = RpcV3.Immediate(PlayerControl.LocalPlayer.NetId, 204).Protected(false).ThreadSafe(true)
            .Write(callId)
            .Write((byte)receivers)
            .WritePacked(PlayerControl.LocalPlayer.NetId)
            .Write(batchId)
            .Write((byte)args.Length);

        RpcBody extractedBody = ((RpcV3)monoRpc).ExtractBody();
        

        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            RpcHookHelper.GlobalSendCount++;
            RpcBody clonedBody = extractedBody.Clone();
            clonedBody.Write((byte)index);
            
            if (arg is IBatchSendable batchSendable)
            {
                BatchWriter batchWriter = new(clonedBody);
                batchSendable.Write(batchWriter);
                batchWriter.BatchRpcs.Do(rpc =>
                {
                    FinalizeV2(targets, monoRpc.SetBody(rpc));
                    Thread.Sleep(20);
                });
                continue;
            }

            clonedBody.Write((byte)0);
            clonedBody.Write(arg!);
            FinalizeV2(targets, monoRpc.SetBody(clonedBody));
            Thread.Sleep(20);
        }

        monoRpc.SetBody(extractedBody);
        monoRpc.Write((byte)0);
        monoRpc.Write((byte)4);
        FinalizeV2(targets, monoRpc);
    }

    private void FinalizeV2(int[]? targets, MonoRpc monoRpc)
    {
        int[]? blockedClients = Vents.CallingAssemblyBlacklist();

        string senderString = AmongUsClient.Instance.AmHost ? "Host" : "NonHost";
        int clientId = PlayerControl.LocalPlayer.GetClientId();
        if (targets != null) {
            log.Debug($"[{localSendCount}::{uuid}::{RpcHookHelper.GlobalSendCount}](Client: {clientId}) Sending RPC ({callId}) as {senderString} to {targets.StrJoin()} | {senders}", "DetouredSender");
            monoRpc.SendInclusive(blockedClients == null ? targets : targets.Except(blockedClients).ToArray());
        } else if (blockedClients != null) {
            log.Debug($"[{localSendCount}::{uuid}::{RpcHookHelper.GlobalSendCount}](Client: {clientId}) Sending RPC ({callId}) as {senderString} to all except {blockedClients.StrJoin()} | {senders}", "DetouredSender");
            monoRpc.SendExcluding(blockedClients);
        } else {
            log.Debug($"[{localSendCount}::{uuid}::{RpcHookHelper.GlobalSendCount}] (Client: {clientId}) Sending RPC ({callId}) as {senderString} to all | {senders}", "DetouredSender");
            monoRpc.Send();
        }
    }

    private bool CanSend(out int[]? targets)
    {
        targets = null;
        if (receivers is RpcActors.LastSender) targets = new[] { Vents.GetLastSender(callId)?.GetClientId() ?? 999 };

        return senders switch
        {
            RpcActors.None => false,
            RpcActors.Host => AmongUsClient.Instance.AmHost,
            RpcActors.NonHosts => !AmongUsClient.Instance.AmHost,
            RpcActors.LastSender => true,
            RpcActors.Everyone => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

