using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class IntInserter: IRpcInserter<int>
{
    public static IntInserter Instance = null!;

    public IntInserter()
    {
        Instance = this;
    }
    
    public void Insert(int value, MessageWriter writer)
    {
        writer.Write(value);
    }
}