using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AmongUs.GameOptions;
using HarmonyLib;
using Hazel;
using InnerNet;
using UnityEngine;
using VentLib.Networking.Interfaces;
using VentLib.Utilities;

namespace VentLib.Networking.RPC;

public class RpcV2
{
    private uint netId;
    private byte callId;
    private bool immediate;
    private bool requireHost;

    private readonly List<(object, WriteType)> writes = new();
    private SendOption sendOption = SendOption.Reliable;

    public static uint GetHostNetId()
    {
        PlayerControl? host = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(player => player.GetClientId() == AmongUsClient.Instance.HostId);
        return host == null ? 0 : host.NetId;
    }

    public static RpcV2 Standard(byte netId, byte callId, SendOption sendOption = SendOption.Reliable)
    {
        return new RpcV2
        {
            netId = netId,
            callId = callId,
            immediate = false,
            sendOption = sendOption
        };
    }

    public static RpcV2 Immediate(uint netId, RpcCalls call, SendOption sendOption = SendOption.Reliable) => Immediate(netId, (byte)call, sendOption);

    public static RpcV2 Immediate(uint netId, byte callId, SendOption sendOption = SendOption.Reliable)
    {
        return new RpcV2
        {
            netId = netId,
            callId = callId,
            immediate = true,
            sendOption = sendOption
        };
    }

    public RpcV2 Write(bool value) => WriteAny(value);
    public RpcV2 Write(byte value) => WriteAny(value);
    public RpcV2 Write(float value) => WriteAny(value);
    public RpcV2 Write(int value) => WriteAny(value);
    public RpcV2 Write(sbyte value) => WriteAny(value);
    public RpcV2 Write(string value) => WriteAny(value);
    public RpcV2 Write(uint value) => WriteAny(value);
    public RpcV2 Write(ulong value) => WriteAny(value);
    public RpcV2 Write(ushort value) => WriteAny(value);
    public RpcV2 Write(Vector2 vector) => WriteAny(vector, WriteType.Vector);
    public RpcV2 Write(InnerNetObject value) => WriteAny(value, WriteType.NetObject);
    public RpcV2 Write(IRpcWritable value) => WriteAny(value, WriteType.Rpcable);
    public RpcV2 Write(IBatchSendable value) => WriteAny(value, WriteType.Rpcable);
    public RpcV2 WritePacked(int value) => WriteAny(value, WriteType.Packed);
    public RpcV2 WritePacked(uint value) => WriteAny(value, WriteType.Packed);
    public RpcV2 WriteSerializable(object value) => WriteAny(value, WriteType.Serializable);

    public RpcV2 WriteOptions(IGameOptions options) => WriteAny(options, WriteType.Options);

    public RpcV2 RequireHost(bool requireHost)
    {
        this.requireHost = requireHost;
        return this;
    }

    public void SendInclusive(params int[] include)
    {
        PlayerControl.AllPlayerControls.ToArray()
            .Where(pc => include.Contains(pc.GetClientId()))
            .Do(pc => Send(pc.GetClientId()));
    }

    public void SendExclusive(params int[] exclude)
    {
        PlayerControl.AllPlayerControls.ToArray()
            .Where(pc => !exclude.Contains(pc.GetClientId()))
            .Do(pc => Send(pc.GetClientId()));
    }

    public void SendToHost()
    {
        netId = (byte)GetHostNetId();
        Send(PlayerControl.LocalPlayer.GetClientId());
    }

    public void SendNet()
    {
        foreach (uint key in AmongUsClient.Instance.allObjectsFast.Keys)
        {
            netId = key;
            Send();
        }
    }

    private static void Serialize(MessageWriter writer, object obj)
    {
        MethodInfo serialzeMethod = obj.GetType().GetMethod("Serialize", BindingFlags.Default | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
        serialzeMethod.Invoke(obj,
            serialzeMethod.GetParameters().Length == 1 ? new object?[] { writer } : new object?[] { writer, false });
    }

    public void WriteTo(MessageWriter writer)
    {
        foreach ((object, WriteType) write in writes)
        {
            switch (write.Item2)
            {
                case WriteType.NetObject:
                    writer.WriteNetObject((InnerNetObject)write.Item1);
                    continue;
                case WriteType.Packed:
                {
                    if (write.Item1 is uint item1) writer.WritePacked(item1);
                    else writer.WritePacked((int)write.Item1);
                    continue;
                }
                case WriteType.Options:
                    writer.WriteBytesAndSize(
                        GameOptionsManager.Instance.gameOptionsFactory.ToBytes((IGameOptions)write.Item1));
                    continue;
                case WriteType.Vector:
                    NetHelpers.WriteVector2((Vector2)write.Item1, writer);
                    continue;
                case WriteType.Rpcable:
                    ((IRpcWritable)write.Item1).Write(writer);
                    continue;
                case WriteType.Serializable:
                    Serialize(writer, write.Item1);
                    continue;
                case WriteType.Normal:
                default:
                    switch (write.Item1)
                    {
                        case bool data:
                            writer.Write(data);
                            break;
                        case byte data:
                            writer.Write(data);
                            break;
                        case float data:
                            writer.Write(data);
                            break;
                        case int data:
                            writer.Write(data);
                            break;
                        case sbyte data:
                            writer.Write(data);
                            break;
                        case string data:
                            writer.Write(data);
                            break;
                        case uint data:
                            writer.Write(data);
                            break;
                        case ulong data:
                            writer.Write(data);
                            break;
                        case ushort data:
                            writer.Write(data);
                            break;
                    }

                    break;
            }
        }
    }

    public void Send(int clientId = -1)
    {
        if (requireHost && !AmongUsClient.Instance.AmHost) return;
        MessageWriter writer = !immediate
            ? AmongUsClient.Instance.StartRpc(netId, callId, sendOption)
            : AmongUsClient.Instance.StartRpcImmediately(netId, callId, sendOption, clientId);

        WriteTo(writer);

        if (!immediate)
            writer.EndMessage();
        else
            AmongUsClient.Instance.FinishRpcImmediately(writer);
    }

    public RpcV2 Clone()
    {
        RpcV2 cloned = new RpcV2 
        {
            netId = netId,
            callId = callId,
            requireHost = requireHost,
            immediate = immediate
        };
        cloned.writes.AddRange(writes);
        return cloned;
    }

    private RpcV2 WriteAny(object value, WriteType writeType = WriteType.Normal)
    {
        writes.Add((value, writeType));
        return this;
    }

    private enum WriteType: byte
    {
        Normal,
        Packed,
        Rpcable,
        NetObject,
        Options,
        Vector,
        Serializable
    }
}