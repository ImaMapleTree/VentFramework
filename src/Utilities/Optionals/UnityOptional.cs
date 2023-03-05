using System;
using Object = UnityEngine.Object;

namespace VentLib.Utilities.Optionals;

public class UnityOptional<T> : Optional<T> where T: Object
{
    public UnityOptional() { }

    public UnityOptional(T? item) : base(item) { }

    public new static UnityOptional<T> Of(T? item)
    {
        return new UnityOptional<T>(item);
    }

    public new static UnityOptional<T> NonNull(T item)
    {
        if (item == null) throw new NullReferenceException($"Item of type {typeof(T)} cannot be null.");
        return new UnityOptional<T>(item);
    }

    public new static UnityOptional<T> Null()
    {
        return new UnityOptional<T>();
    }

    public static UnityOptional<T> From(UnityOptional<T>? optional)
    {
        return optional ?? Null();
    }

    public override bool Exists() => HasValue && Object.IsNativeObjectAlive(Item);

    public UnityOptional<TR> UnityMap<TR>(Func<T, TR> mapFunc) where TR: Object => new(Exists() ? mapFunc(Item!) : default);

    public UnityOptional<TR> FlatMap<TR>(Func<T, UnityOptional<TR>> mapFunc) where TR: Object => Exists() ? UnityOptional<TR>.Of(mapFunc(Item!).Item) : UnityOptional<TR>.Null();

    public override string ToString()
    {
        string unityObject;
        if (Il2CppSystem.Object.ReferenceEquals(Item, null)) unityObject = "";
        else if (!Object.IsNativeObjectAlive(Item)) unityObject = "";
        else unityObject = Item!.ToString();
        return $"UnityOptional({unityObject})";
    }
    
}