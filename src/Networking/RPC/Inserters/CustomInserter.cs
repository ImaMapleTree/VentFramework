using System;
using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class CustomInserter<T>: IRpcInserter<T>
{
    private Action<T, MessageWriter> writerFunction;

    public CustomInserter(Action<T, MessageWriter> writerFunction)
    {
        this.writerFunction = writerFunction;
    }

    public void Insert(T value, MessageWriter writer)
    {
        writerFunction(value, writer);
    }
}