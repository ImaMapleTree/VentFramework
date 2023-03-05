namespace VentLib.Networking.Interfaces;

public interface IRpcSendable<out T>: IRpcReadable<T>, IRpcWritable where T: IRpcReadable<T>
{
}