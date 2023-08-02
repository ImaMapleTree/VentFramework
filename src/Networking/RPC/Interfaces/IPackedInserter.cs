using Hazel;

namespace VentLib.Networking.RPC.Interfaces;

public interface IPackedInserter: IRpcInserter
{
    public void WritePacked(object value, MessageWriter writer);
}

public interface IPackedInserter<in T>: IRpcInserter<T>, IPackedInserter
{
    public void WritePacked(T value, MessageWriter writer);

    void IRpcInserter<T>.Insert(T value, MessageWriter writer) => WritePacked(value, writer);

    void IPackedInserter.WritePacked(object value, MessageWriter writer) => WritePacked((T)value, writer);
}