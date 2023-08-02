namespace VentLib.Utilities.Collections;

public class Remote<T>: IRemote
{
    private uint id;
    private UuidList<T> parentList;
    private bool deleted;

    internal Remote(UuidList<T> parentList, uint id)
    {
        this.id = id;
        this.parentList = parentList;
    }

    public virtual T Get() => parentList.Get(id);

    public virtual bool Delete() => deleted = parentList.Remove(id);

    public virtual bool IsDeleted() => deleted;

    public Remote<T2> Cast<T2>() where T2: class
    {
        return new RefRemote<T, T2>(this, id);
    }

    private class RefRemote<T1, T2> : Remote<T2> where T2: class
    {
        private Remote<T1> originalRemote;
        
        internal RefRemote(Remote<T1> remote, uint id) : base(new UuidList<T2>(), id)
        {
            originalRemote = remote;
        }

        public override T2 Get() => (originalRemote.Get() as T2)!;

        public override bool Delete() => originalRemote.Delete();

        public override bool IsDeleted() => originalRemote.IsDeleted();
    }
}