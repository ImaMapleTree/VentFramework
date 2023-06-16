using System;
using VentLib.Logging;
using VentLib.Utilities.Collections;

namespace VentLib.Networking.RPC;

public static class RpcMonitor
{
    private static bool enabled;
    private static Remote<Action<RpcMeta>> metaRemote;

    public static void Enable()
    {
        enabled = true;
        metaRemote = RpcMeta.AddSubscriber(MonitorRpc);
    }

    private static void MonitorRpc(RpcMeta meta)
    {
        if (!enabled) return;
        if (meta.PacketSize <= NetworkRules.MaxPacketSize) return;
        VentLogger.Warn($"Found RPC exceeding maximum packet size ({meta.PacketSize} > {NetworkRules.MaxPacketSize}). Full Info={meta}");
    }
}