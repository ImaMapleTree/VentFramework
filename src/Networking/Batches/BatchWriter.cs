using System.Collections.Generic;
using AmongUs.GameOptions;
using InnerNet;
using UnityEngine;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC;

namespace VentLib.Networking.Batches;

public sealed class BatchWriter
{
    internal static uint BatchId;
    internal readonly List<RpcV2> BatchRpcs = new();
    private ushort batchNumber;
    private RpcV2 template;

    internal BatchWriter(RpcV2 rpcV2)
    {
        template = rpcV2;
        rpcV2 = template.Clone();
        BatchRpcs.Add(rpcV2);
        rpcV2.Write((byte)1);
        rpcV2.Write((ushort)0);
    }

    public BatchWriter NextBatch()
    {
        RpcV2 rpcV2 = template.Clone();
        BatchRpcs.Add(rpcV2);
        rpcV2.Write((byte)2);
        rpcV2.Write(++batchNumber);
        return this;
    }

    public BatchEnd EndBatch()
    {
        /*RpcV2 rpcV2 = template.Clone();
        rpcV2.Write(3);
        BatchRpcs.Add(rpcV2);*/
        return new BatchEnd();
    }

    public BatchWriter Write(bool value)
    {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(byte value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(float value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(int value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(sbyte value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(string value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(uint value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(ulong value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(ushort value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(Vector2 vector) {
        BatchRpcs[^1].Write(vector);
        return this;
    } 
    public BatchWriter Write(InnerNetObject value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(IRpcWritable value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(IBatchSendable value) {
        BatchRpcs[^1].Write(value);
        return this;
    } 
    public BatchWriter Write(object value) {
        BatchRpcs[^1].WriteSerializable(value);
        return this;
    } 
    public BatchWriter Write(IGameOptions options) {
        BatchRpcs[^1].WriteOptions(options);
        return this;
    } 
    public BatchWriter WritePacked(int value) {
        BatchRpcs[^1].WritePacked(value);
        return this;
    }
    public BatchWriter WritePacked(uint value) {
        BatchRpcs[^1].WritePacked(value);
        return this;
    }
}