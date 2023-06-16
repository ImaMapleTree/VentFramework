using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class UlongInserter: IRpcInserter<ulong>
{
    public static UlongInserter Instance = null!;

    public UlongInserter()
    {
        Instance = this;
    }
    
    public void Insert(ulong value, MessageWriter writer)
    {
        writer.Write(value);
    }
}