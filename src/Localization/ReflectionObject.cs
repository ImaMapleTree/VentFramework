using System;
using System.Reflection;

namespace VentLib.Localization;

internal class ReflectionObject
{
    internal readonly ReflectionType ReflectionType;
    private object Object;

    internal ReflectionObject(object obj, ReflectionType reflectionType)
    {
        Object = obj;
        ReflectionType = reflectionType;
    }

    internal void SetValue(object value)
    {
        switch (ReflectionType)
        {
            case ReflectionType.Class:
                break;
            case ReflectionType.Method:
                break;
            case ReflectionType.StaticField:
                FieldInfo field = (FieldInfo)Object;
                field.SetValue(null, value);
                break;
            case ReflectionType.Property:
            case ReflectionType.InstanceField:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    internal object? GetValue()
    {
        return (ReflectionType) switch
        {
            ReflectionType.Class => null,
            ReflectionType.Method => null,
            ReflectionType.StaticField => ((FieldInfo)Object).GetValue(null),
            ReflectionType.InstanceField => null,
            ReflectionType.Property => null,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}

public enum ReflectionType
{
    Class,
    Method,
    StaticField,
    InstanceField,
    Property
}