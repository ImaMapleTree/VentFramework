using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using VentLib.Utilities.Extensions;
using VentLib.Utilities.Optionals;

namespace VentLib.Utilities.Collections;

public class UuidList<T>: IUuidList<T>
{
    private List<PhantomEntry> items = new();
    public IEnumerator<T> GetEnumerator() => items.Select(pe => pe.Value).GetEnumerator();
    private uint id;

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public uint Add(T item)
    {
        uint uuid = id;
        items.Add(new PhantomEntry(id++, item));
        return uuid;
    }

    public void Clear()
    {
        items.Clear();
        id = 0;
    }

    public bool Contains(T item) => this.Any(i => Equals(item, i));

    public void CopyTo(T[] array, int arrayIndex)
    {
        Array.Copy(items.Select(i => i.Value).ToArray(), 0, array, arrayIndex, items.Count);
    }

    public bool Remove(T item)
    {
        PhantomEntry entry = items.FirstOrDefault(pe => Equals(item, pe.Value));
        return entry.ID != uint.MaxValue && items.Remove(entry);
    }

    public bool Remove(uint uuid)
    {
        PhantomEntry entry = items.FirstOrDefault(pe => pe.ID == uuid);
        return entry.ID != uint.MaxValue && items.Remove(entry);
    }
    
    
    public T? RemoveItem(uint uuid)
    {
        PhantomEntry entry = items.FirstOrDefault(pe => pe.ID == uuid);
        T? item = entry.ID == uint.MaxValue ? default : entry.Value;
        Remove(uuid);
        return item;
    }

    public int RemoveAll(Func<T, bool> predicate) => items.RemoveAll(pi => pi.ID != uint.MaxValue && predicate(pi.Value));

    public int Count => items.Count;

    public bool IsReadOnly => false;

    public T Get(uint uuid) => items.First(pe => pe.ID == uuid).Value;

    public Optional<uint> UuidOf(T item) => items.FirstOrOptional(pe => Equals(pe.Value, item)).Map(pe => pe.ID);
    
    public int IndexOf(T item) => items.FindIndex(pe => Equals(pe.Value, item));

    public int IndexOf(uint uuid) => items.FindIndex(pe => pe.ID == uuid);
    
    public uint Insert(int index, T item)
    {
        uint uuid = id++;
        items.Insert(index, new PhantomEntry(uuid, item));
        return uuid;
    }

    public void RemoveAt(int index) => items.RemoveAt(index);

    public T this[int index]
    {
        get => items[index].Value;
        set => items[index] = new PhantomEntry(id++, value);
    }

    private struct PhantomEntry
    {
        public readonly uint ID = uint.MaxValue;
        public readonly T Value;
        
        public PhantomEntry(uint id, T value)
        {
            ID = id;
            Value = value;
        }
    }
}