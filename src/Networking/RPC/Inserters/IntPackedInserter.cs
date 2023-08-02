using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class IntPackedInserter: IPackedInserter<int>
{
    public static IntPackedInserter Instance = null!;

    public IntPackedInserter()
    {
        Instance = this;
    }

    public void WritePacked(int value, MessageWriter writer)
    {
        writer.WritePacked(value);
    }
}