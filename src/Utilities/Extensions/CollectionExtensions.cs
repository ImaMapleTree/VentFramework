using System;
using System.Collections.Generic;

namespace VentLib.Utilities.Extensions;

public static class CollectionExtensions
{
    public static string StrJoin<T>(this IEnumerable<T> list)
    {
        return "[" + string.Join(", ", list) + "]";
    }

    public static bool TryGet<TKey, TValue>(this Il2CppSystem.Collections.Generic.Dictionary<TKey, TValue> dictionary, TKey key, out TValue? v)
    {
        v = default;
        if (!dictionary.ContainsKey(key)) return false;
        v = dictionary[key];
        return true;
    }
    
    public static T GetRandom<T>(this List<T> list) => list[new Random().Next(list.Count)];
    
    public static T PopRandom<T>(this List<T> list)
    {
        return list.Pop(new Random().Next(list.Count));
    }

    public static T Pop<T>(this List<T> list, int index)
    {
        T value = list[index];
        list.RemoveAt(index);
        return value;
    }

    public static bool Replace<T>(this List<T> list, T target, T replacement)
    {
        int index = list.IndexOf(target);
        if (index == -1) return false;
        list[index] = replacement;
        return true;
    }

    public static void Swap<T>(this List<T> list, int index1, int index2)
    {
        (list[index1], list[index2]) = (list[index2], list[index1]);
    }
}