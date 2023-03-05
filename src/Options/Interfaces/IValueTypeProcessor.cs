using VentLib.Options.IO;

namespace VentLib.Options.Interfaces;

public interface IValueTypeProcessor
{
    public object Read(MonoLine input);

    public MonoLine Write(object value, MonoLine output);
}

public interface IValueTypeProcessor<T> : IValueTypeProcessor
{
    public new T Read(MonoLine input);

    object IValueTypeProcessor.Read(MonoLine input)
    {
        return Read(input)!;
    }

    public MonoLine Write(T value, MonoLine output);

    MonoLine IValueTypeProcessor.Write(object value, MonoLine output)
    {
        return Write((T)value, output);
    }
}