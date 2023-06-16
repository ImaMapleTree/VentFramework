using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace VentLib.Utilities.Collections;

public class OrderedSet<T> : ISet<T>, IList<T>
{
    private HashSet<T> backingSet = new();
    private List<T> backingList = new();

    public OrderedSet()
    {
    }

    public OrderedSet(ICollection<T> collection)
    {
        backingSet = new HashSet<T>(collection);
        backingList = collection.Distinct().ToList();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return backingList.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    void ICollection<T>.Add(T item)
    {
        if (!backingSet.Add(item)) return;
        backingList.Add(item);
    }

    public void ExceptWith(IEnumerable<T> other)
    {
        HashSet<T> otherSet = other.ToHashSet();
        backingList = backingList.Where(i => !otherSet.Contains(i)).Distinct().ToList();
        backingSet = new HashSet<T>(backingList);
    }

    public void IntersectWith(IEnumerable<T> other)
    {
        HashSet<T> otherSet = other.ToHashSet();
        backingList = backingList.Where(i => otherSet.Contains(i)).Distinct().ToList();
        backingSet = new HashSet<T>(backingList);
    }

    public bool IsProperSubsetOf(IEnumerable<T> other) => backingSet.IsProperSubsetOf(other);

    public bool IsProperSupersetOf(IEnumerable<T> other) => backingSet.IsProperSupersetOf(other);

    public bool IsSubsetOf(IEnumerable<T> other) => backingSet.IsSubsetOf(other);

    public bool IsSupersetOf(IEnumerable<T> other) => backingSet.IsSupersetOf(other);

    public bool Overlaps(IEnumerable<T> other) => backingSet.Overlaps(other);

    public bool SetEquals(IEnumerable<T> other) => backingSet.SetEquals(other);

    public void SymmetricExceptWith(IEnumerable<T> other)
    {
        T[] otherArray = other.ToArray();
        HashSet<T> otherSet = otherArray.ToHashSet();
        backingList = backingList.Where(i => !otherSet.Contains(i)).Distinct().ToList();
        backingList.AddRange(otherArray.Where(i => !backingSet.Contains(i)));
        backingSet = new HashSet<T>(backingList);
    }

    public void UnionWith(IEnumerable<T> other)
    {
        backingList.AddRange(other.Distinct());
        backingSet = new HashSet<T>(backingList);
    }

    public bool Add(T item)
    {
        if (!backingSet.Add(item)) return false;
        backingList.Add(item);
        return true;
    }
    
    bool ISet<T>.Add(T item)
    {
        if (!backingSet.Add(item)) return false;
        backingList.Add(item);
        return true;
    }

    public void Clear()
    {
        backingSet.Clear();
        backingList.Clear();
    }

    public bool Contains(T item) => backingSet.Contains(item);

    public void CopyTo(T[] array, int arrayIndex) => backingList.CopyTo(array, arrayIndex);

    public bool Remove(T item)
    {
        if (!backingSet.Remove(item)) return false;
        backingList.Remove(item);
        return true;
    }
    
    public int Count => backingSet.Count;

    public bool IsReadOnly => false;


    public int IndexOf(T item) => backingList.IndexOf(item);

    public void Insert(int index, T item)
    {
        if (!backingSet.Add(item)) return;
        backingList.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        T item = backingList[index];
        backingSet.Remove(item);
        backingList.RemoveAt(index);
    }

    public List<T> AsList() => this.backingList;

    public T this[int index]
    {
        get => backingList[index];
        set
        {
            T oldItem = backingList[index];
            backingSet.Remove(oldItem);
            backingList[index] = value;
        }
    }
}