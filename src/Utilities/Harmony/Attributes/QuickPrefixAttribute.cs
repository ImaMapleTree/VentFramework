using System;

namespace VentLib.Utilities.Harmony.Attributes;

public class QuickPrefixAttribute : QuickHarmonyAttribute
{
    public QuickPrefixAttribute(Type targetType, string methodName, int priority = 1) : base(targetType, methodName, HarmonyPatchType.Prefix, priority)
    {
    }
}