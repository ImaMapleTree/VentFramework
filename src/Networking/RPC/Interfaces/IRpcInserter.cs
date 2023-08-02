using Hazel;

namespace VentLib.Networking.RPC.Interfaces;

public interface IRpcInserter
{
    public void Insert(object value, MessageWriter writer);
}

public interface IRpcInserter<in T>: IRpcInserter
{
    public void Insert(T value, MessageWriter writer);

    void IRpcInserter.Insert(object value, MessageWriter writer) => Insert((T)value, writer);
}