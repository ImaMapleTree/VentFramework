using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Hazel;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class EnumerableInserter: IRpcInserter<IEnumerable>
{
    public void Insert(IEnumerable value, MessageWriter writer)
    {
        List<object> list = value.Cast<object>().ToList();
        writer.Write((ushort)list.Count);
        foreach (object obj in list) RpcBody.GetInserter(obj.GetType()).Insert(obj, writer);
    }
}