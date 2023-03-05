using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using InnerNet;
using UnityEngine;
using VentLib.Logging;
using VentLib.Networking.Batches;
using VentLib.Networking.Helpers;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Networking.RPC;

internal class DetouredSender
{
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

    private async void DelayedSend(int[]? targets, object?[] args)
    {
        int retries = 0;
        while ((AmongUsClient.Instance == null || PlayerControl.LocalPlayer == null) && retries < 50)
        {
            await Task.Delay(500);
            retries++;
        }

        if (retries >= 50)
            VentLogger.Error("Could not gather client instance", "DelayedSend");
        Send(targets, args);
    }

    internal void Send(int[]? targets, object?[] args)
    {
        if (AmongUsClient.Instance == null || PlayerControl.LocalPlayer == null) {
            DelayedSend(targets, args);
            return;
        }
        if (!CanSend(out int[]? lastSender) || !Vents.CallingAssemblyFlag().HasFlag(VentControlFlag.AllowedSender)) return;
        lastSender ??= targets;
        if (batchSend)
            BatchSend(lastSender, args);
        else
            RealSend(lastSender, args);
    }

    private void RealSend(int[]? targets, object?[] args)
    {
        RpcHookHelper.GlobalSendCount++; localSendCount++;
        RpcV2 v2 = RpcV2.Immediate(PlayerControl.LocalPlayer.NetId, 203).Write(callId).RequireHost(false);
        v2.Write((byte)receivers);
        v2.WritePacked(PlayerControl.LocalPlayer.NetId);
        args.Do(a => WriteArg(v2, a!));
        FinalizeV2(targets, v2);
    }

    private void BatchSend(int[]? targets, object?[] args)
    {
        localSendCount++;
        uint batchId = BatchWriter.BatchId++;
        RpcV2 v2 = RpcV2.Immediate(PlayerControl.LocalPlayer.NetId, 204).Write(callId).RequireHost(false);
        v2.Write((byte)receivers);
        v2.WritePacked(PlayerControl.LocalPlayer.NetId);
        v2.Write(batchId);
        v2.Write((byte)args.Length);
        
        for (var index = 0; index < args.Length; index++)
        {
            var arg = args[index];
            RpcHookHelper.GlobalSendCount++;
            RpcV2 cloned = v2.Clone();
            cloned.Write((byte)index);
            
            if (arg is IBatchSendable batchSendable)
            {
                BatchWriter batchWriter = new BatchWriter(cloned);
                batchSendable.Write(batchWriter);
                batchWriter.BatchRpcs.Do(rpc =>
                {
                    FinalizeV2(targets, rpc);
                    Thread.Sleep(20);
                });
                continue;
            }

            cloned.Write((byte)0);
            WriteArg(cloned, arg!);
            FinalizeV2(targets, cloned);
            Thread.Sleep(20);
        }

        v2.Write((byte)0);
        v2.Write((byte)4);
        FinalizeV2(targets, v2);
    }

    private void FinalizeV2(int[]? targets, RpcV2 v2)
    {
        int[]? blockedClients = Vents.CallingAssemblyBlacklist();

        string senderString = AmongUsClient.Instance.AmHost ? "Host" : "NonHost";
        int clientId = PlayerControl.LocalPlayer.GetClientId();
        if (targets != null) {
            VentLogger.Debug($"[{localSendCount}::{uuid}::{RpcHookHelper.GlobalSendCount}](Client: {clientId}) Sending RPC ({callId}) as {senderString} to {targets.StrJoin()} | {senders}", "DetouredSender");
            v2.SendInclusive(blockedClients == null ? targets : targets.Except(blockedClients).ToArray());
        } else if (blockedClients != null) {
            VentLogger.Debug($"[{localSendCount}::{uuid}::{RpcHookHelper.GlobalSendCount}](Client: {clientId}) Sending RPC ({callId}) as {senderString} to all except {blockedClients.StrJoin()} | {senders}", "DetouredSender");
            v2.SendExclusive(blockedClients);
        } else {
            VentLogger.Debug($"[{localSendCount}::{uuid}::{RpcHookHelper.GlobalSendCount}] (Client: {clientId}) Sending RPC ({callId}) as {senderString} to all | {senders}", "DetouredSender");
            v2.Send();
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

    internal static void WriteArg(RpcV2 rpcV2, object arg)
    {
        RpcV2 _ = (arg) switch
        {
            bool data => rpcV2.Write(data),
            byte data => rpcV2.Write(data),
            float data => rpcV2.Write(data),
            int data => rpcV2.Write(data),
            sbyte data => rpcV2.Write(data),
            string data => rpcV2.Write(data),
            uint data => rpcV2.Write(data),
            ulong data => rpcV2.Write(data),
            ushort data => rpcV2.Write(data),
            Vector2 data => rpcV2.Write(data),
            InnerNetObject data => rpcV2.Write(data),
            IRpcWritable data => rpcV2.Write(data),
            _ => WriteArgNS(rpcV2, arg)
        };
    }

    private static RpcV2 WriteArgNS(RpcV2 rpcV2, object arg)
    {
        switch (arg)
        {
            case IEnumerable enumerable:
                List<object> list = enumerable.Cast<object>().ToList();
                rpcV2.Write((ushort)list.Count);
                foreach (object obj in list)
                    WriteArg(rpcV2, obj);
                break;
            default:
                throw new ArgumentOutOfRangeException($"Invalid Argument: {arg}");
        }

        return rpcV2;
    }
}

