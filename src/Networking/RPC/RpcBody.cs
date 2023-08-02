using System;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using InnerNet;
using VentLib.Logging;
using VentLib.Networking.Interfaces;
using VentLib.Networking.RPC.Inserters;
using VentLib.Networking.RPC.Interfaces;
using VentLib.Utilities;

namespace VentLib.Networking.RPC;

public class RpcBody
{
    private static StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(RpcBody));
    private static readonly Dictionary<Type, IRpcInserter> Inserters = new();
    private static readonly Dictionary<Type, IPackedInserter> PackedInserters = new();

    internal List<object> Arguments = new();
    private List<IRpcInserter> inserters = new();

    static RpcBody()
    {
        RegisterDefaultInserters();
    }
    
    
    public static RpcBody Of(object obj)
    {
        RpcBody rpcBody = new();
        return rpcBody.Write(obj);
    }

    public RpcBody Write(object obj)
    {
        WriteInternal(obj);
        return this;
    }

    public RpcBody Write(IRpcWritable obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj), "Cannot write null objects!");
        Arguments.Add(obj);
        inserters.Add(RpcWritableInserter.Instance);
        return this;
    }
    
    public RpcBody Write(bool b)
    {
        Arguments.Add(b);
        inserters.Add(BoolInserter.Instance);
        return this;
    }
    
    public RpcBody Write(byte b)
    {
        Arguments.Add(b);
        inserters.Add(ByteInserter.Instance);
        return this;
    }
    
    public RpcBody Write(float f)
    {
        Arguments.Add(f);
        inserters.Add(FloatInserter.Instance);
        return this;
    }
    
    public RpcBody Write(int i)
    {
        Arguments.Add(i);
        inserters.Add(IntInserter.Instance);
        return this;
    }
    
    public RpcBody Write(sbyte sb)
    {
        Arguments.Add(sb);
        inserters.Add(SbyteInserter.Instance);
        return this;
    }
    
    public RpcBody Write(uint ui)
    {
        Arguments.Add(ui);
        inserters.Add(UintInserter.Instance);
        return this;
    }

    public RpcBody Write(ulong ul)
    {
        Arguments.Add(ul);
        inserters.Add(UlongInserter.Instance);
        return this;
    }
    
    public RpcBody Write(ushort ush)
    {
        Arguments.Add(ush);
        inserters.Add(UshortInserter.Instance);
        return this;
    }

    public RpcBody Write(InnerNetObject innerNetObject)
    {
        Arguments.Add(innerNetObject);
        inserters.Add(InnerNetObjectInserter.Instance);
        return this;
    }

    public RpcBody WriteCustom<T>(T obj, IRpcInserter customInserter)
    {
        Arguments.Add(obj!);
        inserters.Add(customInserter);
        return this;
    }

    public RpcBody WritePacked(object obj)
    {
        WriteInternalPacked(obj);
        return this;
    }

    public RpcBody WritePacked(int i)
    {
        Arguments.Add(i);
        inserters.Add(IntPackedInserter.Instance);
        return this;
    }
    
    public RpcBody WritePacked(uint i)
    {
        Arguments.Add(i);
        inserters.Add(UintPackedInserter.Instance);
        return this;
    }

    private void WriteInternal(object? obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj), "Cannot write null objects!");
        Type objectType = obj.GetType();
        IRpcInserter inserter = GetInserter(objectType);
        Arguments.Add(obj);
        inserters.Add(inserter);
    }
    
    private void WriteInternalPacked(object? obj)
    {
        if (obj == null) throw new ArgumentNullException(nameof(obj), "Cannot write null objects!");
        Type objectType = obj.GetType();
        IPackedInserter? inserter = PackedInserters.GetValueOrDefault(objectType);
        if (inserter == null)
        {
            (Type key, inserter) = PackedInserters.FirstOrDefault(t => t.Key.IsAssignableFrom(objectType));
            if (inserter != null) log.Warn($"Found inserter of inherited-type \"{key}\". Please consider registering \"{objectType.Name}\" with the same inserter.");
            else throw new VentLibException($"Cannot write object. {nameof(IPackedInserter)} does not exist for type {obj.GetType()}.");
        }
            
        Arguments.Add(obj);
        inserters.Add(inserter);
    }

    public void WriteAll(MessageWriter writer)
    {
        for (int index = 0; index < Arguments.Count; index++)
        {
            IRpcInserter inserter = inserters[index];
            if (inserter is IPackedInserter packedInserter) packedInserter.WritePacked(Arguments[index], writer);
            else inserter.Insert(Arguments[index], writer);
        }
    }

    public static void RegisterInserter(Type type, IRpcInserter inserter) => Inserters.Add(type, inserter);

    public static void RegisterInserter<T>(IRpcInserter<T> inserter) => Inserters.Add(typeof(T), inserter);
    
    public static void RegisterPackedInserter(Type type, IPackedInserter inserter)
    {
        PackedInserters[type] = inserter;
    }

    public static void RegisterPackedInserter<T>(IPackedInserter<T> inserter)
    {
        Type t = typeof(T);
        PackedInserters[t] = inserter;
    }

    public static void SetInserter(Type type, Type typeWithInserter)
    {
        Inserters[type] = Inserters[typeWithInserter];
    }

    public static void SetPackedInserter(Type type, Type typeWithPackedInserter)
    {
        PackedInserters[type] = PackedInserters[typeWithPackedInserter];
    }

    public static IRpcInserter GetInserter(Type type)
    {
        IRpcInserter? inserter = Inserters.GetValueOrDefault(type);
        if (inserter != null) return inserter;
        
        if (type.IsAssignableTo(typeof(IRpcWritable))) return Inserters[typeof(IRpcWritable)];
        
        (Type key, inserter) = Inserters.FirstOrDefault(t => t.Key.IsAssignableFrom(type));
        if (inserter != null) log.Warn($"Found inserter of inherited-type \"{key}\". Please consider registering \"{type.Name}\" with the same inserter.");
        else throw new VentLibException($"Cannot write object. {nameof(IRpcInserter)} does not exist for type {type}.");
        return inserter;
    }
    
    public RpcBody Clone()
    {
        return new RpcBody
        {
            Arguments = new List<object>(Arguments),
            inserters = new List<IRpcInserter>(inserters)
        };
    }

    private static void RegisterDefaultInserters()
    {
        RegisterInserter(new BoolInserter());
        RegisterInserter(new ByteInserter());
        RegisterInserter(new FloatInserter());
        RegisterInserter(new GameOptionsInserter());
        RegisterInserter(new InnerNetObjectInserter());
        RegisterInserter(new IntInserter());
        RegisterPackedInserter(new IntPackedInserter());
        RegisterInserter(new RpcWritableInserter());
        RegisterInserter(new SbyteInserter());
        RegisterInserter(new StringInserter());
        RegisterInserter(new UintInserter());
        RegisterPackedInserter(new UintPackedInserter());
        RegisterInserter(new UlongInserter());
        RegisterInserter(new UshortInserter());
        RegisterInserter(new Vector2Inserter());
        RegisterInserter(new EnumerableInserter());
        SetInserter(typeof(PlayerControl), typeof(InnerNetObject));
    }
}