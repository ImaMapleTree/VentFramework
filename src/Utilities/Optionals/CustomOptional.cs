using System;

namespace VentLib.Utilities.Optionals;

public class CustomOptional<T> : Optional<T>
{
    private readonly Func<T?, bool> exists;

    private CustomOptional(Func<T, bool>? checkFunction)
    {
        checkFunction ??= _ => true;
        exists = checkFunction!;
    }

    private CustomOptional(T? item, Func<T, bool>? checkFunction) : base(item)
    {
        checkFunction ??= _ => true;
        exists = checkFunction!;
    }

    public static CustomOptional<T> Of(T? item, Func<T, bool>? checkFunction = null)
    {
        checkFunction ??= _ => true;
        return new CustomOptional<T>(item, checkFunction);
    }

    public static CustomOptional<T> NonNull(T item, Func<T, bool>? checkFunction = null)
    {
        if (item == null) throw new NullReferenceException($"Item of type {typeof(T)} cannot be null.");
        return new CustomOptional<T>(item, checkFunction);
    }

    public static CustomOptional<T> Null(Func<T, bool>? checkFunction = null)
    {
        return new CustomOptional<T>(checkFunction);
    }

    public static CustomOptional<T> From(Optional<T>? optional, Func<T, bool>? checkFunction = null)
    {
        return new CustomOptional<T>(optional?.Exists() ?? false ? optional.Get() : default, checkFunction);
    }

    public override bool Exists()
    {
        return HasValue && exists(Item);
    }

    public CustomOptional<TR> CustomMap<TR>(Func<T, TR> mapFunc, Func<TR, bool>? checkFunc = null)
    {
        return new(Exists() ? mapFunc(Item!) : default, checkFunc);
    }

    public CustomOptional<TR> FlatMap<TR>(Func<T, CustomOptional<TR>> mapFunc, Func<TR, bool>? checkFunc = null)
    {
        return Exists() ? CustomOptional<TR>.Of(mapFunc(Item!).Item, checkFunc) : CustomOptional<TR>.Null();
    }

    public override string ToString()
    {
        string unityObject;
        if (ReferenceEquals(Item, null)) unityObject = "";
        else if (!exists(Item)) unityObject = "";
        else unityObject = Item!.ToString()!;
        return $"CustomOptional({unityObject})";
    }
    
}