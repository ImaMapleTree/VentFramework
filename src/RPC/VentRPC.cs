#nullable enable
using System.Reflection;
using VentLib.Logging;
using VentLib.Utilities;

// ReSharper disable RedundantAssignment

namespace VentLib.RPC;

public static class VentRPC
{
    [VentRPC(VentCall.SetControlFlag, RpcActors.Host, RpcActors.NonHosts)]
    public static void SetControlFlag(string assemblyName, int controlFlag)
    {
        VentLogger.Trace($"SetControlFlag(assemblyName={assemblyName}, controlFlag={controlFlag})", "VentLib");
        Assembly? assembly = AssemblyUtils.FindAssemblyFromFullName(assemblyName);
        if (assembly == null) return;
        Vents.SetControlFlag(assembly, (VentControlFlag)controlFlag);
        VentLogger.Trace($"Control Flag Set For: {assembly.GetName().Name} | Flag: {(VentControlFlag)controlFlag})");
    }
}

public enum VentCall: uint
{
    VersionCheck = 1017,
    SetControlFlag = 1018
}