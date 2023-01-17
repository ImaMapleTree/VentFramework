namespace VentLib.RPC.Interfaces;

public interface IRpcSendable<out T>: IRpcReadable<T>, IRpcWritable where T: IRpcReadable<T>
{
}