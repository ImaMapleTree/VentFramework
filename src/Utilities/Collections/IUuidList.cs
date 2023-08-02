using System.Collections.Generic;

namespace VentLib.Utilities.Collections;

public interface IUuidList<T>: IList<T>
{
    void ICollection<T>.Add(T item)
    {
        Add(item);
    }

    new uint Add(T item);

    void IList<T>.Insert(int index, T item)
    {
        Insert(index, item);
    }

    new uint Insert(int index, T item);
}