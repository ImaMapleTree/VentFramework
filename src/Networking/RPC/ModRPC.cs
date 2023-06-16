#nullable enable
using System;
using System.Reflection;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using VentLib.Logging;
using VentLib.Networking.Helpers;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities.Extensions;

namespace VentLib.Networking.RPC;

public class ModRPC
{
    public readonly uint CallId;
    public RpcActors Senders { get; }
    public RpcActors Receivers { get; }
    public MethodInvocation Invocation { get; }
    internal readonly Type[] Parameters;
    internal readonly Assembly? Assembly;
    internal readonly MethodInfo TargetMethod;
    internal readonly ModRPCAttribute Attribute;
    internal DetouredSender Sender = null!;
    private readonly MethodBase trampoline;
    private readonly Func<object?> instanceSupplier;

    internal ModRPC(ModRPCAttribute attribute, MethodInfo targetMethod)
    {
        Attribute = attribute;
        TargetMethod = targetMethod;
        CallId = attribute.CallId;
        Senders = attribute.Senders;
        Receivers = attribute.Receivers;
        Invocation = attribute.Invocation;
        Parameters = ParameterHelper.Verify(targetMethod.GetParameters());
        Type? declaringType = targetMethod.DeclaringType;
        if (declaringType == null)
            throw new ArgumentException($"Unable to Register: {targetMethod.Name}. Reason: VentLib does not current allow for methods without declaring types");

        Assembly = declaringType.Assembly;
        Hook hook = RpcHookHelper.Generate(this);
        trampoline = hook.GenerateTrampoline();

        instanceSupplier = () =>
        {
            if (targetMethod.IsStatic) return null;
            if (!IRpcInstance.Instances.TryGetValue(declaringType, out IRpcInstance? instance))
                throw new NullReferenceException($"Cannot invoke non-static method because IRpcInstance.EnableInstance() was never called for {declaringType}");
            return instance;
        };
    }

    public void Send(int[]? clientIds, params object[] args)
    {
        Sender.Send(clientIds, args);
    }

    public void InvokeTrampoline(params object[] args)
    {
        VentLogger.Log(LogLevel.All,$"Calling trampoline \"{trampoline.FullDescription()}\" with args: {args.StrJoin()}", "RPCTrampoline");
        trampoline.Invoke(instanceSupplier(), args);
    }
}