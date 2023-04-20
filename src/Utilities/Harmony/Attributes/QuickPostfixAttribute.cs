using System;

namespace VentLib.Utilities.Harmony.Attributes;

public class QuickPostfixAttribute : QuickHarmonyAttribute
{
    public QuickPostfixAttribute(Type targetType, string methodName, int priority = 1) : base(targetType, methodName, HarmonyPatchType.Postfix, priority)
    {
    }
}