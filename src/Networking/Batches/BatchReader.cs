using System;
using System.Data;
using System.Reflection;
using HarmonyLib;
using Hazel;
using VentLib.Logging;

namespace VentLib.Networking.Batches;

public sealed class BatchReader
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(BatchReader));
    private readonly MessageReader[] readerBuffer = new MessageReader[65535];
    private int realCapacity;
    private int readerIndex;

    public bool HasNext() => readerIndex < realCapacity;

    internal BatchReader(MessageReader initialReader)
    {
        Add(initialReader);
    }

    public MessageReader GetNext()
    {
        if (readerIndex - 1 >= 0)
            readerBuffer[readerIndex-1].Recycle();
        return readerBuffer[readerIndex++];
    } 

    internal void Add(MessageReader reader)
    {
        int index = reader.ReadUInt16();
        readerBuffer[index] = MessageReader.Get(reader);
        realCapacity++;
    }

    internal object Initialize(Type targetType)
    {
        log.Trace($"Initializing Batch Type {targetType}");
        ConstructorInfo? constructor = AccessTools.Constructor(targetType);
        if (constructor == null) 
            throw new ConstraintException($"Unable to initialize BatchType: {targetType}. Class must have a default no-args constructor.");
        
        object batchObject = constructor.Invoke(null);
        MethodInfo method = AccessTools.Method(targetType, "Read", new[] { typeof(BatchReader) })!;
        return method.Invoke(batchObject, new object[] { this })!;
    }
}