extern alias JetbrainsAnnotations;
using System;
using System.Collections.Generic;
using JetbrainsAnnotations::JetBrains.Annotations;
using VentLib.Utilities.Optionals;

namespace VentLib.Utilities.Extensions;

public static class DictionaryExtensions
{
    public static TValue GetOrCompute<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue> supplier) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key)) return dictionary[key] = supplier();
        return dictionary[key];
    }

    public static Optional<TValue> GetOptional<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
    {
        return !dictionary.TryGetValue(key, out TValue? value) ? Optional<TValue>.Null() : Optional<TValue>.NonNull(value);
    }

    [UsedImplicitly]
    private static TValue? GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key) where TKey : notnull
    {
        return dictionary.TryGetValue(key, out TValue? value) ? value : default;
    }

    /// <summary>
    /// Composes a value in a dictionary, updating the value if the key already exists, otherwise setting the value for the key
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="updateFunction"></param>
    /// <param name="providerFunction"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TValue Compose<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> updateFunction, Func<TValue> providerFunction) where TKey : notnull
    {
        if (!dictionary.ContainsKey(key)) return dictionary[key] = providerFunction();
        return dictionary[key] = updateFunction(dictionary[key]);
    }
    
    /// <summary>
    /// Updates a value in a dictionary, given it exists.
    /// </summary>
    /// <param name="dictionary"></param>
    /// <param name="key"></param>
    /// <param name="updateFunction"></param>
    /// <typeparam name="TKey"></typeparam>
    /// <typeparam name="TValue"></typeparam>
    /// <returns></returns>
    public static TValue Compose<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TValue, TValue> updateFunction) where TKey : notnull
    {
        return dictionary[key] = updateFunction(dictionary[key]);
    }
}