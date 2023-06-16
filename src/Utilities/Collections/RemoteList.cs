namespace VentLib.Utilities.Collections;

public class RemoteList<T> : UuidList<T>
{
    public new Remote<T> Add(T item)
    {
        return new Remote<T>(this, base.Add(item));
    }

    public new Remote<T> Insert(int index, T item)
    {
        return new Remote<T>(this, base.Insert(index, item));
    }

    public Remote<T>? GetRemote(int index)
    {
        return GetRemote(base[index]);
    }

    public Remote<T>? GetRemote(T? item)
    {
        if (item == null) return null;
        return UuidOf(item).Transform(id => new Remote<T>(this, id), () => null!);
    }
}