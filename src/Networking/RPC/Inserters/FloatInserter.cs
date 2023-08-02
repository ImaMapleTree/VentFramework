using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class FloatInserter: IRpcInserter<float>
{
    public static FloatInserter Instance = null!;

    public FloatInserter()
    {
        Instance = this;
    }
    
    public void Insert(float value, MessageWriter writer)
    {
        writer.Write(value);
    }
}