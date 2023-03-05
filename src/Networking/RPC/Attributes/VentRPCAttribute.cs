namespace VentLib.Networking.RPC.Attributes;

public class VentRPCAttribute: ModRPCAttribute
{
    public VentRPCAttribute(VentCall call, RpcActors senders = RpcActors.Everyone, RpcActors receivers = RpcActors.Everyone, MethodInvocation invocation = MethodInvocation.ExecuteNever) : base((uint)call, senders, receivers, invocation) { }
}