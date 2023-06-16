using System.Collections.Generic;

namespace VentLib.Utilities.Extensions;

public static class Il2CppHashSetExtensions
{
    public static List<T> ToList<T>(this Il2CppSystem.Collections.Generic.HashSet<T> hashSet)
    {
        List<T> list = new();
        foreach (T item in hashSet) list.Add(item);
        return list;
    } 
}