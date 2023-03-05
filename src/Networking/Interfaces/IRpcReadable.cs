using Hazel;

namespace VentLib.Networking.Interfaces;

public interface IRpcReadable<out T> where T: IRpcReadable<T>
{
    T Read(MessageReader reader);
}