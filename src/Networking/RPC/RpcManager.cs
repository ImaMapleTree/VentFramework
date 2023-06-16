using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using VentLib.Logging;
using VentLib.Networking.Batches;
using VentLib.Networking.Helpers;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;
using VentLib.Utilities.Extensions;

namespace VentLib.Networking.RPC;

internal static class RpcManager
{
    private static readonly Dictionary<uint, object[]> BatchArgumentStorage = new();

    internal static void Register(ModRPC rpc)
    {
        if (Vents.BuiltinRPCs.Contains(rpc.CallId) && rpc.Attribute is not VentRPCAttribute)
            throw new ArgumentException($"RPC {rpc.CallId} shares an ID with a Builtin-VentLib RPC. Please choose a different ID. (Builtin-IDs: {Vents.BuiltinRPCs.StrJoin()})");

        if (!Vents.RpcBindings.ContainsKey(rpc.CallId))
            Vents.RpcBindings.Add(rpc.CallId, new List<ModRPC>());

        Vents.RpcBindings[rpc.CallId].Add(rpc);
    }

    internal static void HandleRpc(byte callId, MessageReader reader)
    {
        if (callId is not (203 or 204)) return;
        uint customId = reader.ReadUInt32();
        RpcActors actor = (RpcActors)reader.ReadByte();
        if (!CanReceive(actor)) return;
        uint senderId = reader.ReadPackedUInt32();
        
        PlayerControl? player = null;
        if (AmongUsClient.Instance.allObjectsFast.TryGet(senderId, out InnerNetObject? netObject))
        {
            player = netObject!.TryCast<PlayerControl>();
            if (player != null) Vents.LastSenders[customId] = player;
        }

        if (player != null && player.PlayerId == PlayerControl.LocalPlayer.PlayerId) return;
        string sender = "Client: " + (player == null ? "?" : player.GetClientId());
        string receiverType = AmongUsClient.Instance.AmHost ? "Host" : "NonHost";
        VentLogger.Info($"Custom RPC Received ({customId}) from \"{sender}\" as {receiverType}", "VentLib");
        if (!Vents.RpcBindings.TryGetValue(customId, out List<ModRPC>? rpcs))
        {
            VentLogger.Warn($"Received Unknown RPC: {customId}", "VentLib");
            return;
        }

        if (rpcs.Count == 0) return;


        object[]? args = null;
        if (callId == 204)
        {
            if (!HandleBatch(reader, rpcs[0], out object[] batchArgs)) return;
            args = batchArgs;
        }

        foreach (ModRPC modRPC in rpcs)
        {
            // Cases in which the client is not the correct listener
            if (!CanReceive(actor, modRPC.Receivers)) continue;
            if (!Vents.CallingAssemblyFlag(modRPC.Assembly).HasFlag(VentControlFlag.AllowedReceiver)) continue;
            args ??= ParameterHelper.Cast(modRPC.Parameters, reader);
            modRPC.InvokeTrampoline(args);
        }
    }

    private static bool HandleBatch(MessageReader reader, ModRPC rpc, out object[] args)
    {
        uint batchId = reader.ReadUInt32();
        byte argumentAmount = reader.ReadByte();
        BatchArgumentStorage.TryAdd(batchId, new object[argumentAmount]);

        args = BatchArgumentStorage[batchId];
        byte argumentIndex = reader.ReadByte();
        byte batchMarker = reader.ReadByte();
        VentLogger.Trace($"Handling Batch (ID={batchId}, Marker={batchMarker}, Index={argumentIndex}, Args={argumentAmount})", "BatchRPC");
        if (batchMarker == 4)
        {
            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] is BatchReader batchReaderArg)
                    args[i] = batchReaderArg.Initialize(rpc.Parameters[i]);
            }

            BatchArgumentStorage.Remove(batchId);
            return true;
        }

        
        if (args[argumentIndex] is not BatchReader batchReader) 
            args[argumentIndex] = reader.ReadDynamic(rpc.Parameters[argumentIndex]);
        else batchReader.Add(reader);
        return false;
    }

    private static bool CanReceive(RpcActors actor, RpcActors localActor = RpcActors.Everyone)
    {
        return actor switch
        {
            RpcActors.None => false,
            RpcActors.Host => AmongUsClient.Instance.AmHost && localActor is RpcActors.Host or RpcActors.NonHosts,
            RpcActors.NonHosts => !AmongUsClient.Instance.AmHost && localActor is RpcActors.Everyone or RpcActors.NonHosts,
            RpcActors.LastSender => localActor is RpcActors.Everyone or RpcActors.LastSender,
            RpcActors.Everyone => true,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

