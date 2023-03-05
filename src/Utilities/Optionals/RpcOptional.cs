using System;
using Hazel;
using VentLib.Networking.Helpers;
using VentLib.Networking.Interfaces;

namespace VentLib.Utilities.Optionals;

public class RpcOptional<T> : Optional<T>, IRpcSendable<RpcOptional<T>> where T : IRpcSendable<T>
{
    private RpcOptional() {
    }

    private RpcOptional(T? item) : base(item) {
    }

    private RpcOptional(MessageReader reader)
    {
        if (reader.ReadByte() == 0) return;
        Item = reader.ReadDynamic(typeof(T));
        HasValue = true;
    }
    
    public new static RpcOptional<T> Of(T? item)
    {
        return new RpcOptional<T>(item);
    }

    public new static RpcOptional<T> NonNull(T item)
    {
        if (item == null) throw new NullReferenceException($"Item of type {typeof(T)} cannot be null.");
        return new RpcOptional<T>(item);
    }

    public new static RpcOptional<T> Null()
    {
        return new RpcOptional<T>();
    }

    public RpcOptional<T> Read(MessageReader reader)
    {
        return new RpcOptional<T>(reader);
    }

    public void Write(MessageWriter writer)
    {
        byte exists = Exists() ? (byte)1 : (byte)0;
        writer.Write(exists);
        if (exists == 0) return;
        Item!.Write(writer);
    }
}