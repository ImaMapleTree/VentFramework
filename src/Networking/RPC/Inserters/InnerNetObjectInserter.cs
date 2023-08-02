using Hazel;
using InnerNet;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class InnerNetObjectInserter: IRpcInserter<InnerNetObject>
{
    public static InnerNetObjectInserter Instance = null!;

    public InnerNetObjectInserter()
    {
        Instance = this;
    }
    
    public void Insert(InnerNetObject value, MessageWriter writer)
    {
        writer.WriteNetObject(value);
    }
}