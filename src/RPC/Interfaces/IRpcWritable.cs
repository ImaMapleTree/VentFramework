using Hazel;

namespace VentLib.RPC.Interfaces;

public interface IRpcWritable
{
    void Write(MessageWriter writer);
}