#nullable enable
using System.Reflection;
using VentLib.Logging;
using VentLib.Networking.RPC.Attributes;
using VentLib.Utilities;

// ReSharper disable RedundantAssignment

namespace VentLib.Networking.RPC;

public static class VentRPC
{
    [VentRPC(VentCall.SetControlFlag, RpcActors.Host, RpcActors.NonHosts)]
    public static void SetControlFlag(string assemblyName, int controlFlag)
    {
        VentLogger.Trace($"SetControlFlag(assemblyName={assemblyName}, controlFlag={controlFlag})", "SetControlFlag");
        Assembly? assembly = AssemblyUtils.FindAssemblyFromFullName(assemblyName);
        if (assembly == null) return;
        Vents.SetControlFlag(assembly, (VentControlFlag)controlFlag);
    }
}

public enum VentCall: uint
{
    VersionCheck = 1017,
    SetControlFlag = 1018
}