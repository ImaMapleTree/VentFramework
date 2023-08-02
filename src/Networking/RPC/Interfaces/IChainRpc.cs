namespace VentLib.Networking.RPC.Interfaces;

public interface IChainRpc: IStrongRpc
{
    public IChainRpc Write(object obj);

    public IChainRpc WritePacked(object obj);

    public MassRpc End();
}