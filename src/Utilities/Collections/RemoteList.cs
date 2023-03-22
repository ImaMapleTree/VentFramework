namespace VentLib.Utilities.Collections;

public class RemoteList<T> : UuidList<T>
{
    public new Remote<T> Add(T item)
    {
        return new Remote<T>(this, base.Add(item));
    }
}