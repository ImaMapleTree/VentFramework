using System.Collections.Generic;
using VentLib.Networking.RPC;

namespace VentLib.Networking.Batches;

public sealed class BatchWriter
{
    internal static uint BatchId;
    internal readonly List<RpcBody> BatchRpcs = new();
    private ushort batchNumber;
    private RpcBody template;

    internal BatchWriter(RpcBody rpcV2)
    {
        template = rpcV2;
        rpcV2 = template.Clone();
        BatchRpcs.Add(rpcV2);
        rpcV2.Write((byte)1);
        rpcV2.Write((ushort)0);
    }

    public BatchWriter NextBatch()
    {
        RpcBody rpcV2 = template.Clone();
        BatchRpcs.Add(rpcV2);
        rpcV2.Write((byte)2);
        rpcV2.Write(++batchNumber);
        return this;
    }

    public BatchEnd EndBatch()
    {
        return new BatchEnd();
    }
    
    public BatchWriter Write(object value) {
        BatchRpcs[^1].Write(value);
        return this;
    }

    public BatchWriter WritePacked(object value)
    {
        BatchRpcs[^1].WritePacked(value);
        return this;
    }
}