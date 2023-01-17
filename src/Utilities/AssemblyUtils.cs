using System;
using System.Linq;
using System.Reflection;

namespace VentLib.Utilities;

public class AssemblyUtils
{
    public static Assembly? FindAssemblyFromFullName(string? fullName)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().FullName == fullName);
    }
    
    public static Assembly? FindAssemblyFromSimpleName(string? simpleName)
    {
        return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == simpleName);
    }
}