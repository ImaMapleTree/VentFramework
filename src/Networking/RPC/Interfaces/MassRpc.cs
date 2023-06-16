namespace VentLib.Networking.RPC.Interfaces;

// ReSharper disable once InconsistentNaming
public interface MassRpc: IStrongRpc
{
    public IChainRpc Start(uint netId, byte callId);

    public IChainRpc Start(uint netId, RpcCalls callId) => Start(netId, (byte)callId);

    public MassRpc Add(IStrongRpc rpc);

    public void Send(int clientId = -1);

    public void SendInclusive(params int[] clientIds);

    public void SendExcluding(params int[] clientIds);
}