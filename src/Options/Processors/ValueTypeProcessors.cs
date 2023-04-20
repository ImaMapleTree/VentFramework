using System;
using System.Collections.Generic;
using VentLib.Options.Interfaces;
using VentLib.Options.IO;
using VentLib.Utilities.Attributes;

namespace VentLib.Options.Processors;

[LoadStatic]
public class ValueTypeProcessors
{
    private static Dictionary<Type, IValueTypeProcessor> _typeProcessors = new();
    
    static ValueTypeProcessors()
    {
        AddTypeProcessor(new BoolProcessor());
        AddTypeProcessor(new ByteProcessor());
        AddTypeProcessor(new SbyteProcessor());
        AddTypeProcessor(new ShortProcessor());
        AddTypeProcessor(new UshortProcessor());
        AddTypeProcessor(new IntProcessor());
        AddTypeProcessor(new UintProcessor());
        AddTypeProcessor(new LongProcessor());
        AddTypeProcessor(new UlongProcessor());
        AddTypeProcessor(new FloatProcessor());
        AddTypeProcessor(new DoubleProcessor());
        AddTypeProcessor(new DecimalProcessor());
        AddTypeProcessor(new StringProcessor());
    }

    public static void AddTypeProcessor(IValueTypeProcessor typeProcessor, Type type)
    {
        _typeProcessors[type] = typeProcessor;
    }

    public static void AddTypeProcessor<T>(IValueTypeProcessor<T> typeProcessor)
    {
        _typeProcessors[typeof(T)] = typeProcessor;
    }

    public static string WriteToString(object value)
    {
        IValueTypeProcessor? processor = _typeProcessors.GetValueOrDefault(value.GetType());
        if (processor == null) throw new NullReferenceException($"No Value Processor exists for type {value.GetType()}");
        return processor.Write(value, new MonoLine()).Content;
    }

    public static object ReadFromLine(string line, Type expectedType) => ReadFromLine(new MonoLine(line), expectedType);

    internal static object ReadFromLine(MonoLine line, Type expectedType)
    {
        IValueTypeProcessor? processor = _typeProcessors.GetValueOrDefault(expectedType);
        if (processor == null) throw new NullReferenceException($"No Value Processor exists for type {expectedType}");
        return processor.Read(line);
    }
}