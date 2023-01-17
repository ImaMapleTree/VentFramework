using System;
using System.Collections.Generic;
using System.Reflection;
using VentLib.Logging;

namespace VentLib.Localization;

public class FrozenContext
{
    private Assembly assembly;
    private readonly object translateTarget;
    private readonly string assemblyName;
    private readonly LocalizedAttribute? parentAttribute;

    public FrozenContext(object target)
    {
        Type? declaringType = new System.Diagnostics.StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType;
        translateTarget = target;
        assembly = Assembly.GetCallingAssembly();
        assemblyName = assembly.GetName().Name!;
        parentAttribute = declaringType?.GetCustomAttribute<LocalizedAttribute>();
    }

    public string Resolve()
    {
        switch (translateTarget)
        {
            case string str:
                return Localizer.Get(GetPath(str), assemblyName);
            case IList<string> li:
                ResolveList(li);
                break;
            default:
                throw new ArgumentException($"Cannot Translate Type {translateTarget.GetType()}");
        }

        return null!;
    }

    private void ResolveList(IList<string> list)
    {
        for (int i = 0; i < list.Count; i++)
            list[i] = Localizer.Get(GetPath(list[i]));
    }

    private string GetPath(string key)
    {
        string? root = parentAttribute?.GetPath();
        return root == null ? key : $"{root}.{key}";
    }
}