using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using VentLib.Logging;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Harmony.Attributes;
using HarmonyPatchType = VentLib.Utilities.Harmony.Attributes.HarmonyPatchType;

namespace VentLib.Utilities.Harmony;

public class HarmonyQuickPatcher
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(HarmonyQuickPatcher));
    private static Dictionary<Assembly, HarmonyLib.Harmony> _harmonyInstances = new();

    public static void ApplyHarmonyPatches(Assembly assembly)
    {
        var pseudoHarmony = _harmonyInstances.GetOrCompute(assembly, () => new HarmonyLib.Harmony(assembly.FullName));

        assembly.GetTypes()
            .SelectMany(type => type.GetMethods(AccessFlags.StaticAccessFlags))
            .Where(t => t.GetCustomAttribute<QuickHarmonyAttribute>() != null)
            .ForEach(method =>
            {
                QuickHarmonyAttribute harmonyAttribute = method.GetCustomAttribute<QuickHarmonyAttribute>()!;
                HarmonyMethod harmonyMethod = new(method, priority: harmonyAttribute.Priority);
                MethodBase targetMethod = AccessTools.Method(harmonyAttribute.TargetType, harmonyAttribute.MethodName);

                log.Trace($"Quick Patching => {targetMethod.Name} ({harmonyAttribute.TargetType})", "HarmonyQuickPatcher");
                
                switch (harmonyAttribute.PatchType)
                {
                    case HarmonyPatchType.Prefix:
                        pseudoHarmony.Patch(targetMethod, prefix: harmonyMethod);
                        break;
                    case HarmonyPatchType.Postfix:
                        pseudoHarmony.Patch(targetMethod, postfix: harmonyMethod);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            });
    }

    public static void UnapplyHarmonyPatches(Assembly assembly)
    {
        var pseudoHarmony = _harmonyInstances.GetOrCompute(assembly, () => new HarmonyLib.Harmony(assembly.FullName));
        pseudoHarmony.UnpatchSelf();
    }
}