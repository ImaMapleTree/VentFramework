using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class ByteInserter: IRpcInserter<byte>
{
    public static ByteInserter Instance = null!;

    public ByteInserter()
    {
        Instance = this;
    }
    
    public void Insert(byte value, MessageWriter writer)
    {
        writer.Write(value);
    }
}