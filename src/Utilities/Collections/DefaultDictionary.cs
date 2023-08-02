using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace VentLib.Utilities.Collections;

public class DefaultDictionary<TKey, TValue>: Dictionary<TKey, TValue> where TKey : notnull
{
    private readonly Func<TKey, TValue> valueSupplier;
    
    public DefaultDictionary(Func<TKey, TValue> valueSupplier)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(Func<TValue> valueSupplier)
    {
        this.valueSupplier = _ => valueSupplier();
    }

    
    
    public DefaultDictionary(IDictionary<TKey, TValue> dictionary, Func<TKey, TValue> valueSupplier) : base(dictionary)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(IDictionary<TKey, TValue> dictionary, Func<TValue> valueSupplier) : base(dictionary)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    

    public DefaultDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer, Func<TKey, TValue> valueSupplier) : base(dictionary, comparer)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(IDictionary<TKey, TValue> dictionary, IEqualityComparer<TKey>? comparer, Func<TValue> valueSupplier) : base(dictionary, comparer)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    
    

    public DefaultDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TKey, TValue> valueSupplier) : base(collection)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, Func<TValue> valueSupplier) : base(collection)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    
    

    public DefaultDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer, Func<TValue> valueSupplier) : base(collection, comparer)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    public DefaultDictionary(IEnumerable<KeyValuePair<TKey, TValue>> collection, IEqualityComparer<TKey>? comparer, Func<TKey, TValue> valueSupplier) : base(collection, comparer)
    {
        this.valueSupplier = valueSupplier;
    }
    
    
    

    public DefaultDictionary(IEqualityComparer<TKey>? comparer, Func<TKey, TValue> valueSupplier) : base(comparer)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(IEqualityComparer<TKey>? comparer, Func<TValue> valueSupplier) : base(comparer)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    
    

    public DefaultDictionary(int capacity, Func<TKey, TValue> valueSupplier) : base(capacity)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(int capacity, Func<TValue> valueSupplier) : base(capacity)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    
    

    public DefaultDictionary(int capacity, IEqualityComparer<TKey>? comparer, Func<TKey, TValue> valueSupplier) : base(capacity, comparer)
    {
        this.valueSupplier = valueSupplier;
    }
    
    public DefaultDictionary(int capacity, IEqualityComparer<TKey>? comparer, Func<TValue> valueSupplier) : base(capacity, comparer)
    {
        this.valueSupplier = _ => valueSupplier();
    }
    
    
    

    protected DefaultDictionary(SerializationInfo info, StreamingContext context, Func<TKey, TValue> valueSupplier) : base(info, context)
    {
        this.valueSupplier = valueSupplier;
    }
    
    protected DefaultDictionary(SerializationInfo info, StreamingContext context, Func<TValue> valueSupplier) : base(info, context)
    {
        this.valueSupplier = _ => valueSupplier();
    }

    public TValue Get(TKey key)
    {
        if (!this.ContainsKey(key)) return base[key] = valueSupplier(key);
        return base[key];
    }
    
    public new TValue this[TKey key]
    {
        get { return Get(key); }
        set => base[key] = value;
    }
}