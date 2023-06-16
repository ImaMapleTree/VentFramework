extern alias JetbrainsAnnotations;
using System;
using JetbrainsAnnotations::JetBrains.Annotations;

namespace VentLib.Utilities.Harmony.Attributes;

[MeansImplicitUse]
public class QuickPrefixAttribute : QuickHarmonyAttribute
{
    public QuickPrefixAttribute(Type targetType, string methodName, int priority = HarmonyLib.Priority.Normal) : base(targetType, methodName, HarmonyPatchType.Prefix, priority)
    {
    }
}