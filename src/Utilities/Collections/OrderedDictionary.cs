using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using VentLib.Ranges;

namespace VentLib.Utilities.Collections;

public class OrderedDictionary<TKey, TValue> : IOrderedDictionary where TKey: notnull
{
    private OrderedDictionary implementation;

    public OrderedDictionary()
    {
        implementation = new OrderedDictionary();
    }

    public OrderedDictionary(OrderedDictionary dictionary)
    {
        implementation = dictionary;
    }

    public void Add(object key, object? value)
    {
        implementation.Add(key, value);
    }

    public void Add(TKey key, TValue? value)
    {
        implementation.Add(key, value);
    }

    public void Clear()
    {
        implementation.Clear();
    }

    public bool Contains(object key)
    {
        return implementation.Contains(key);
    }

    public bool ContainsKey(TKey key)
    {
        return implementation.Contains(key);
    }

    public bool ContainsValue(TValue? value)
    {
        return implementation.Values.Cast<TValue>().Contains(value);
    }

    IDictionaryEnumerator IOrderedDictionary.GetEnumerator()
    {
        return implementation.GetEnumerator();
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> Entries()
    {
        List<TKey> keys = GetKeys().ToList();
        List<TValue> values = GetValues().ToList();
        return new IntRangeGen(0, Count - 1).AsEnumerable().Select(i => new KeyValuePair<TKey, TValue>(keys[i], values[i]));
    }

    public void Insert(int index, object key, object? value)
    {
        implementation.Insert(index, key, value);
    }

    public void Insert(int index, TKey key, TValue? value)
    {
        implementation.Insert(index, key, value);
    }

    public void RemoveAt(int index)
    {
        implementation.RemoveAt(index);
    }

    public object? this[int index]
    {
        get => implementation[index];
        set => implementation[index] = value;
    }

    IDictionaryEnumerator IDictionary.GetEnumerator()
    {
        return implementation.GetEnumerator();
    }

    public void Remove(object key)
    {
        implementation.Remove(key);
    }

    public void Remove(TKey key)
    {
        implementation.Remove(key);
    }

    public bool IsFixedSize => implementation.IsReadOnly;

    public bool IsReadOnly => implementation.IsReadOnly;

    public object? this[object key]
    {
        get => implementation[key];
        set => implementation[key] = value;
    }
    
    public TValue? this[TKey key]
    {
        get => (TValue?)implementation[key];
        set => implementation[key] = value;
    }

    public ICollection Keys => implementation.Keys;

    public IEnumerable<TKey> GetKeys() => Keys.Cast<TKey>();

    public ICollection Values => implementation.Values;

    public IEnumerable<TValue> GetValues() => Values.Cast<TValue>();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return implementation.GetEnumerator();
    }

    public void CopyTo(Array array, int index)
    {
        implementation.CopyTo(array, index);
    }

    public int Count => implementation.Count;

    public bool IsSynchronized => false;

    public object SyncRoot => typeof(OrderedDictionary).GetProperty("SyncRoot", AccessFlags.InstanceAccessFlags)!.GetValue(implementation)!;
}