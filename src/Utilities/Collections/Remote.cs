namespace VentLib.Utilities.Collections;

public class Remote<T>
{
    private uint id;
    private UuidList<T> parentList;
    private bool deleted;

    internal Remote(UuidList<T> parentList, uint id)
    {
        this.id = id;
        this.parentList = parentList;
    }

    public T Get() => parentList.Get(id);

    public bool Delete() => deleted = parentList.Remove(id);

    public bool IsDeleted() => deleted;
}