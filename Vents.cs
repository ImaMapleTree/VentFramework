using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using VentLib.Commands;
using VentLib.Localization;
using VentLib.Logging;
using VentLib.Logging.Default;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC;
using VentLib.Networking.RPC.Attributes;
using VentLib.Options;
using VentLib.Utilities;
using VentLib.Utilities.Attributes;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Harmony;
using VentLib.Version;

namespace VentLib;

//if the client has an unsupported addon it's rpcs get disabled completely CHECK!
//if the client is missing an addon then the host's rpcs from that addon to that client get disabled

public class Vents
{
    public static readonly uint[] BuiltinRPCs = Enum.GetValues<VentCall>().Select(rpc => (uint)rpc).ToArray();
    internal static VersionControl VersionControl { get; } = new();
    public static CommandRunner CommandRunner = new();
    
    internal static Assembly RootAssemby = null!;
    internal static Harmony Harmony = new("com.tealeaf.VentLib");
    internal static readonly Dictionary<uint, List<ModRPC>> RpcBindings = new();
    internal static readonly Dictionary<Assembly, VentControlFlag> RegisteredAssemblies = new();
    internal static readonly Dictionary<Assembly, string> AssemblyNames = new();
    internal static readonly Dictionary<Assembly, int[]?> BlockedReceivers = new();
    internal static readonly Dictionary<uint, PlayerControl> LastSenders = new();
    
    private static bool _initialized;

    static Vents()
    {
        BepInExLogListener.BindToUnity();
    }

    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static ModRPC? FindRPC(uint callId, MethodInfo? targetMethod = null)
    {
        if (!RpcBindings.TryGetValue(callId, out List<ModRPC>? RPCs))
        {
            NoDepLogger.Warn($"Attempted to find unregistered RPC: {callId}", "VentLib");
            return null;
        }

        return RPCs.FirstOrDefault(v => targetMethod == null || v.TargetMethod.Equals(targetMethod));
    }
    
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    public static ModRPC? FindRPC(uint callId, Type declaringClass, string methodName, Type[]? parameters = null)
    {
        MethodInfo? method = AccessTools.Method(declaringClass, methodName, parameters);
        if (method == null)
            throw new NullReferenceException($"No matching method with name {methodName} in class {declaringClass}");
        if (!RpcBindings.TryGetValue(callId, out List<ModRPC>? RPCs))
        {
            NoDepLogger.Warn($"Attempted to find unregistered RPC: {callId}", "VentLib");
            return null;
        }

        return RPCs.FirstOrDefault(v => v.TargetMethod.Equals(method));
    }

    public static PlayerControl? GetLastSender(uint rpcId) => LastSenders.GetValueOrDefault(rpcId);

    public static bool Register(Assembly assembly, bool localize = true)
    {
        NoDepLogger.Info($"Registering {assembly.GetName().Name}");
        if (RegisteredAssemblies.ContainsKey(assembly)) return false;
        RegisteredAssemblies.Add(assembly, VentControlFlag.AllowedReceiver | VentControlFlag.AllowedSender);
        AssemblyNames.TryAdd(assembly, assembly.GetName().Name!);
        
        RegisterInIl2CppAttribute.Register(assembly);


        LoadStatic.LoadStaticTypes(assembly);
        HarmonyQuickPatcher.ApplyHarmonyPatches(assembly);
        if (localize) Localizer.Get(assembly);

        OptionManager.GetManager(assembly);
        CommandRunner.Register(assembly);


        var methods = assembly.GetTypes()
            .SelectMany(t => t.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public))
            .Where(m => m.GetCustomAttribute<ModRPCAttribute>() != null).ToList();

        NoDepLogger.Info($"Registering {methods.Count} methods from {assembly.GetName().Name}", "VentLib");
        foreach (var method in methods)
        {
            ModRPCAttribute attribute = method.GetCustomAttribute<ModRPCAttribute>()!;
            Type? declaringType = method.DeclaringType;

            if (!method.IsStatic && declaringType != null && !declaringType.IsAssignableTo(typeof(IRpcInstance)))
                throw new ArgumentException($"Unable to Register Method {method.Name}. Reason: Declaring Class of non-static methods must implement IRpcInstance");

            RpcManager.Register(new ModRPC(attribute, method));
        }
        
        if (localize) Localizer.Localizers.Values.ForEach(locale =>
        {
            locale.Languages.Values.Where(lang => lang.Updated).ForEach(lang => lang.Dump(locale.Serializer));
        });

        return true;
    }

    public static void Initialize()
    {
        if (_initialized) return;
        MainThreadAnchor.IsMainThread();

        NoDepLogger.High($"Initializing VentFramework {Assembly.GetExecutingAssembly().GetName().Version!.ToString(4)} by Tealeaf");
        
        var _ = Async.AUCWrapper;
        RootAssemby = Assembly.GetCallingAssembly();
        IL2CPPChainloader.Instance.PluginLoad += (_, assembly, _) => Register(assembly, assembly == RootAssemby);
        Register(Assembly.GetExecutingAssembly());
        Harmony.PatchAll(Assembly.GetExecutingAssembly());
        _initialized = true;
    }
    
    public static void BlockClient(Assembly assembly, int clientId)
    {
        int[] newBlockedArray = BlockedReceivers.TryGetValue(assembly, out int[]? blockedClients)
            ? blockedClients.AddToArray(clientId)
            : new[] { clientId };
        BlockedReceivers[assembly] = newBlockedArray;
    }

    public static void SetAssemblyRefName(Assembly assembly, string name)
    {
        AssemblyNames[assembly] = name;
    }
    
    internal static int[]? CallingAssemblyBlacklist() => BlockedReceivers.GetValueOrDefault(Assembly.GetCallingAssembly());

    internal static VentControlFlag CallingAssemblyFlag(Assembly? assembly = null)
    {
        if (!RegisteredAssemblies.TryGetValue(assembly ?? Assembly.GetCallingAssembly(), out VentControlFlag flag))
            flag = VentControlFlag.AllowedReceiver | VentControlFlag.AllowedSender;
        return flag;
    }

    internal static void SetControlFlag(Assembly assembly, VentControlFlag flag)
    {
        // Assemblies must be registered first before they can be updated
        if (!RegisteredAssemblies.ContainsKey(assembly)) return;
        RegisteredAssemblies[assembly] = flag;
    }
}

[Flags]
public enum VentControlFlag
{
    AllowedReceiver = 1,
    AllowedSender = 2,
}