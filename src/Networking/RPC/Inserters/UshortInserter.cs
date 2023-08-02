using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class UshortInserter: IRpcInserter<ushort>
{
    public static UshortInserter Instance = null!;

    public UshortInserter()
    {
        Instance = this;
    }
    
    public void Insert(ushort value, MessageWriter writer)
    {
        writer.Write(value);
    }
}