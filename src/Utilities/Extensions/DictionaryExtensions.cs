using System;
using System.Collections.Generic;

namespace VentLib.Utilities.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCompute<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> supplier) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key)) return dictionary[key] = supplier();
        return dictionary[key];
    }
}