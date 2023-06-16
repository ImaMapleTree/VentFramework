using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class BoolInserter: IRpcInserter<bool>
{
    public static BoolInserter Instance = null!;

    public BoolInserter()
    {
        Instance = this;
    }
    
    public void Insert(bool value, MessageWriter writer)
    {
        writer.Write(value);
    }
}