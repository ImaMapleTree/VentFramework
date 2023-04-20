using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;

namespace VentLib.Utilities;

public static class AssemblyUtils
{
    public static Assembly? FindAssemblyFromFullName(string? fullName)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().FullName == fullName);
    }
    
    public static Assembly? FindAssemblyFromSimpleName(string? simpleName)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == simpleName);
    }

    public static Type[] FlattenAssemblyTypes(Assembly assembly, BindingFlags flags)
    {
        Type[] GetTypes(Type type) => type.GetNestedTypes(flags).SelectMany(GetTypes).AddItem(type).ToArray();

        List<Type> surfaceTypes = assembly.GetTypes().ToList();

        return surfaceTypes.SelectMany(GetTypes).Concat(surfaceTypes).ToArray();
    }

    internal static string GetAssemblyRefName(Assembly assembly)
    {
        return assembly == Vents.RootAssemby ? "root" : Vents.AssemblyNames.GetValueOrDefault(assembly, assembly.GetName().Name!);
    }
    
}