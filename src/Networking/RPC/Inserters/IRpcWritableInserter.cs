using Hazel;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class RpcWritableInserter: IRpcInserter<IRpcWritable>
{
    public static RpcWritableInserter Instance = null!;

    public RpcWritableInserter()
    {
        Instance = this;
    }
    
    public void Insert(IRpcWritable value, MessageWriter writer)
    {
        value.Write(writer);
    }
}