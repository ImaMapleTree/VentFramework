using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class UintInserter: IRpcInserter<uint>
{
    public static UintInserter Instance = null!;

    public UintInserter()
    {
        Instance = this;
    }
    
    public void Insert(uint value, MessageWriter writer)
    {
        writer.Write(value);
    }
}