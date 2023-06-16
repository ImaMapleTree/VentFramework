using System;
using System.Collections.Generic;
using System.Data;
using Hazel;
using UnityEngine;
using VentLib.Networking;
using VentLib.Networking.Batches;
using VentLib.Networking.Helpers;
using VentLib.Networking.Interfaces;
using VentLib.Utilities.Optionals;

namespace VentLib.Utilities.Collections;

public class BatchList<T>: List<T>, IBatchSendable<BatchList<T>> where T: IRpcSendable<T>
{
    public BatchList()
    {
    }

    public BatchList(IEnumerable<T> collection): base(collection)
    {
    }

    private Optional<int> itemSize = Optional<int>.Null();

    public void SetItemSize(int bytes)
    {
        itemSize = Optional<int>.NonNull(bytes);
    }

    public BatchEnd Write(BatchWriter batchWriter)
    {
        batchWriter.Write(Count);
        if (Count == 0) return batchWriter.EndBatch();
        itemSize.OrElseSet(() =>
        {
            T sample = this[0];
            MessageWriter writer = MessageWriter.Get();
            sample.Write(writer);
            return writer.Length;
        });

        int length = itemSize.Get();
        if (length == 0) length = 1;
        if (length > NetworkRules.MaxPacketSize - 24)
            throw new ConstraintException($"Length of singular item cannot exceed max packet size ({length} > {NetworkRules.MaxPacketSize})");
        int batchItemCount = (NetworkRules.MaxPacketSize - 24) / length;
        int batches = Mathf.CeilToInt((float)Count / batchItemCount);
        if (batches > 65534) throw new ConstraintException($"BatchList exceeds maximum number of batches ({batches} > 65534)");

        int itemIndex = 0;
        for (int i = 0; i < batches; i++)
        {
            batchWriter = batchWriter.NextBatch();
            batchWriter.Write(Math.Min(batchItemCount, Count - itemIndex));
            for (int j = 0; j < batchItemCount && itemIndex < Count; j++)
            {
                batchWriter.Write(this[itemIndex++]);
            }
        }

        return batchWriter.EndBatch();
    }

    public BatchList<T> Read(BatchReader batchReader)
    {
        MessageReader reader = batchReader.GetNext();
        int total = reader.ReadInt32();
        if (total == 0) return this;

        Type itemType = typeof(T);
        while (batchReader.HasNext())
        {
            reader = batchReader.GetNext();
            int batchItemCount = reader.ReadInt32();
            for (int i = 0; i < batchItemCount; i++)
            {
                T item = reader.ReadDynamic(itemType);
                Add(item);
            }
        }
        return this;
    }
}