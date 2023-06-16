extern alias JetbrainsAnnotations;
using System;
using JetbrainsAnnotations::JetBrains.Annotations;

namespace VentLib.Utilities.Harmony.Attributes;

[MeansImplicitUse]
[AttributeUsage(AttributeTargets.Method, Inherited = false)]
public class QuickHarmonyAttribute : Attribute
{
    public Type TargetType;
    public string MethodName;
    public HarmonyPatchType PatchType;
    public int Priority;

    public QuickHarmonyAttribute(Type targetType, string methodName, HarmonyPatchType patchType, int priority = HarmonyLib.Priority.Normal)
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