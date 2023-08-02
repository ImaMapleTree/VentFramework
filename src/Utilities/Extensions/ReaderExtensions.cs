using System;
using System.Collections.Generic;
using Hazel;
using VentLib.Networking.Helpers;
using VentLib.Networking.RPC;

namespace VentLib.Utilities.Extensions;

public static class ReaderExtensions
{
    public static void WriteList<T>(this MessageWriter writer, List<T> list)
    {
        if (!ParameterHelper.IsTypeAllowed(typeof(T)))
            throw new ArgumentException($"Unable to write list of type {typeof(T)}");
        
        RpcBody.GetInserter(list.GetType()).Insert(list, writer);
    }

    public static List<T> ReadList<T>(this MessageReader reader)
    {
        if (!ParameterHelper.IsTypeAllowed(typeof(T)))
            throw new ArgumentException($"Unable to read list of type {typeof(T)}");
        return (List<T>)reader.ReadDynamic(typeof(List<T>));
    }
}