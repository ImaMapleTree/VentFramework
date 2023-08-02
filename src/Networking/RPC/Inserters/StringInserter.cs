using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class StringInserter: IRpcInserter<string>
{
    public static StringInserter Instance = null!;

    public StringInserter()
    {
        Instance = this;
    }
    
    public void Insert(string value, MessageWriter writer)
    {
        writer.Write(value);
    }
}