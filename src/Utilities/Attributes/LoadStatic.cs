using System;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using VentLib.Logging;

namespace VentLib.Utilities.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public class LoadStatic: Attribute
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(LoadStatic));
    
    internal static void LoadStaticTypes(Assembly assembly)
    {
        assembly.GetTypes().Where(t => t.GetCustomAttribute<LoadStatic>() != null)
            .Do(t =>
            {
                var constructor = t.GetConstructor(BindingFlags.Public | BindingFlags.Static | BindingFlags.NonPublic,
                    Array.Empty<Type>());
                if (constructor == null) return;
                constructor.Invoke(null, null);
                log.Trace($"Statically Initialized Class {t}", "LoadStatic");
            });
    }
}