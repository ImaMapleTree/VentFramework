using System;

namespace VentLib.Utilities.Optionals;

public class Optional<T>
{
    protected T? Item { get; set; }
    protected bool HasValue;

    public Optional()
    {
        Item = default;
    }

    public Optional(T? item)
    {
        Item = item;
        HasValue = item != null;
    }

    public Optional(Optional<T> optional)
    {
        Item = optional.Item;
        if (optional.Exists()) HasValue = true;
    }

    public static Optional<T> Of(T? item)
    {
        return new Optional<T>(item);
    }

    public static Optional<T> NonNull(T item)
    {
        if (item == null) throw new NullReferenceException($"Item of type {typeof(T)} cannot be null.");
        return new Optional<T>(item);
    }

    public static Optional<T> Null()
    {
        return new Optional<T>();
    }

    public static Optional<T> From(Optional<T>? optional)
    {
        return optional == null ? new Optional<T>() : new Optional<T>(optional);
    }

    public virtual bool Exists() => HasValue;

    public Optional<TR> Map<TR>(Func<T, TR> mapFunc) => new(Exists() ? mapFunc(Item!) : default) { HasValue = HasValue };

    public Optional<TR> FlatMap<TR>(Func<T, Optional<TR>> mapFunc) => Exists() ? Optional<TR>.Of(mapFunc(Item!).Item ?? default) : Optional<TR>.Null();

    public TR Transform<TR>(Func<T, TR> exists, Func<TR> otherwise) => Exists() ? exists(Item!) : otherwise();
    
    public void Handle(Action<T> consumer, Action otherwise)
    {
        if (Exists()) consumer(Item!);
        else otherwise();
    }
    
    public void IfPresent(Action<T> consumer)
    {
        if (Exists()) consumer(Item!);
    }
    
    public void IfNotPresent(Action action)
    {
        if (!Exists()) action();
    }

    public T OrElseSet(Func<T> supplier)
    {
        if (Exists()) return Item!;
        Item = supplier();
        HasValue = true;
        return Item!;
    }

    public T Get()
    {
        if (!Exists()) throw new NullReferenceException("Called .Get() on Optional with no item.");
        return Item!;
    }

    public T OrElse(T other) => Exists() ? Item! : other;

    public T OrElseGet(Func<T> supplier) => Exists() ? Item! : supplier();

    public override string ToString()
    {
        return $"Optional({Item?.ToString() ?? ""})";
    }
}