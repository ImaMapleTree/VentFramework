using System;

namespace VentLib.Networking.RPC.Attributes;


[AttributeUsage(AttributeTargets.Method)]
public class ModRPCAttribute : Attribute
{
    public readonly uint CallId;
    public RpcActors Senders { get; }
    public RpcActors Receivers { get; }
    public MethodInvocation Invocation { get; }

    public ModRPCAttribute(uint call, RpcActors senders = RpcActors.Everyone, RpcActors receivers = RpcActors.Everyone, MethodInvocation invocation = MethodInvocation.ExecuteNever)
    {
        CallId = call;
        Senders = senders;
        Receivers = receivers;
        Invocation = invocation;
    }
    
    public ModRPCAttribute(object call, RpcActors senders = RpcActors.Everyone, RpcActors receivers = RpcActors.Everyone, MethodInvocation invocation = MethodInvocation.ExecuteNever)
        : this((uint)call, senders, receivers, invocation)
    {
    }
}

public enum RpcActors: byte
{
    None,
    /// <summary>
    /// Permits ONLY Host to Send / Receive marked RPC
    /// </summary>
    Host,
    /// <summary>
    /// Permits everyone BUT host to Send / Receive marked RPC
    /// </summary>
    NonHosts,
    /// <summary>
    /// Is equivalent to marking as "Everyone"
    /// </summary>
    LastSender,
    Everyone
}

public enum MethodInvocation
{
    ExecuteNever,
    ExecuteBefore,
    ExecuteAfter
}
