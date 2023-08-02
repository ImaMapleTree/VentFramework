using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class SbyteInserter: IRpcInserter<sbyte>
{
    public static SbyteInserter Instance = null!;

    public SbyteInserter()
    {
        Instance = this;
    }
    
    public void Insert(sbyte value, MessageWriter writer)
    {
        writer.Write(value);
    }
}