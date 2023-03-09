using System;
using System.Collections.Generic;
using VentLib.Utilities.Optionals;

namespace VentLib.Utilities.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCompute<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, Func<TValue> supplier) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key)) return dictionary[key] = supplier();
        return dictionary[key];
    }

    public static Optional<TValue> GetOptional<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
    {
        return Optional<TValue>.Of(dictionary.GetValueOrDefault(key));
    } 
}