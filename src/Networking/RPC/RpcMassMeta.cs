using System.Collections.Generic;

namespace VentLib.Networking.RPC;

public class RpcMassMeta: RpcMeta
{
    public List<RpcMeta> ChildMeta { get; internal set; } = null!;
}