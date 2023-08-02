using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using MonoMod.RuntimeDetour;
using VentLib.Networking.RPC;

namespace VentLib.Networking.Helpers;

internal class RpcHookHelper
{
    internal static long GlobalSendCount;
    private static List<DetouredSender> _senders = new();

    private static readonly OpCode[] Ldc = { OpCodes.Ldc_I4_0, OpCodes.Ldc_I4_1, OpCodes.Ldc_I4_2, OpCodes.Ldc_I4_3, OpCodes.Ldc_I4_4, OpCodes.Ldc_I4_5, OpCodes.Ldc_I4_6, OpCodes.Ldc_I4_7, OpCodes.Ldc_I4_8 };
    private static readonly OpCode[] Ldarg = { OpCodes.Ldarg_0, OpCodes.Ldarg_1, OpCodes.Ldarg_2, OpCodes.Ldarg_3 };

    internal static Hook Generate(ModRPC modRPC)
    {
        MethodInfo executingMethod = modRPC.TargetMethod;
        Type[] parameters = executingMethod.GetParameters().Select(p => p.ParameterType).ToArray();

        DynamicMethod m = new(
            executingMethod.Name,
            executingMethod.ReturnType,
            parameters);

        int senderSize = _senders.Count;

        ILGenerator ilg = m.GetILGenerator();
        if (senderSize <= 8)
            ilg.Emit(Ldc[senderSize]);
        else
            ilg.Emit(OpCodes.Ldc_I4_S, senderSize);
        ilg.Emit(OpCodes.Call, AccessTools.Method(typeof(RpcHookHelper), nameof(GetSender)));

        if (parameters.Length <= 8)
            ilg.Emit(Ldc[parameters.Length]);
        else
            ilg.Emit(OpCodes.Ldc_I4_S, parameters.Length);
        ilg.Emit(OpCodes.Newarr, typeof(object));

        for (int i = 0; i < parameters.Length; i++)
        {
            ilg.Emit(OpCodes.Dup);
            if (i <= 8)
                ilg.Emit(Ldc[i]);
            else
                ilg.Emit(OpCodes.Ldc_I4_S, i);

            if (i <= 3)
                ilg.Emit(Ldarg[i]);
            else
                ilg.Emit(OpCodes.Ldarg_S, i);
            if (parameters[i].IsPrimitive)
                ilg.Emit(OpCodes.Box, parameters[i]);
            ilg.Emit(OpCodes.Stelem_Ref);
        }

        ilg.Emit(OpCodes.Callvirt, AccessTools.Method(typeof(DetouredSender), nameof(DetouredSender.IntermediateSend)));
        ilg.Emit(OpCodes.Ret);

        _senders.Add(new DetouredSender(modRPC));
        return new Hook(executingMethod, m);
    }

    private static DetouredSender GetSender(int index) => _senders[index];
}