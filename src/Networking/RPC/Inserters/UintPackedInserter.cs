using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class UintPackedInserter: IPackedInserter<uint>
{
    public static UintPackedInserter Instance = null!;

    public UintPackedInserter()
    {
        Instance = this;
    }

    public void WritePacked(uint value, MessageWriter writer)
    {
        writer.WritePacked(value);
    }
}