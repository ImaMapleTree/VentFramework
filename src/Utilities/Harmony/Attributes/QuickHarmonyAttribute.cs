using System;

namespace VentLib.Utilities.Harmony.Attributes;

[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class QuickHarmonyAttribute : Attribute
{
    public Type TargetType;
    public string MethodName;
    public HarmonyPatchType PatchType;
    public int Priority;

    public QuickHarmonyAttribute(Type targetType, string methodName, HarmonyPatchType patchType, int priority = 1)
    {
        TargetType = targetType;
        MethodName = methodName;
        PatchType = patchType;
        Priority = priority;
    }
}

public enum HarmonyPatchType
{
    Prefix,
    Postfix
}