using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

    internal static string GetAssemblyRefName(Assembly assembly)
    {
        return assembly == Vents.RootAssemby ? "root" : Vents.AssemblyNames.GetValueOrDefault(assembly, assembly.GetName().Name!);
    }
}