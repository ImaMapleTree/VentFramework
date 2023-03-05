using Hazel;

namespace VentLib.Networking.Interfaces;

public interface IRpcWritable
{
    void Write(MessageWriter writer);
}