using Hazel;
using UnityEngine;
using VentLib.Networking.RPC.Interfaces;

namespace VentLib.Networking.RPC.Inserters;

public class Vector2Inserter: IRpcInserter<Vector2>
{
    public void Insert(Vector2 value, MessageWriter writer)
    {
        NetHelpers.WriteVector2(value, writer);
    }
}