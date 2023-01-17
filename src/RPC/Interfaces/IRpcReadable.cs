using Hazel;

namespace VentLib.RPC.Interfaces;

public interface IRpcReadable<out T> where T: IRpcReadable<T>
{
    T Read(MessageReader reader);
}