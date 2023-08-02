using System;
using VentLib.Logging;
using VentLib.Utilities.Collections;

namespace VentLib.Networking.RPC;

public static class RpcMonitor
{
    private static readonly StandardLogger log = LoggerFactory.GetLogger<StandardLogger>(typeof(RpcMonitor));
    private static bool _enabled;
    // ReSharper disable once NotAccessedField.Local
    private static Remote<Action<RpcMeta>>? _metaRemote;

    public static void Enable()
    {
        _enabled = true;
        _metaRemote = RpcMeta.AddSubscriber(MonitorRpc);
    }

    private static void MonitorRpc(RpcMeta meta)
    {
        if (!_enabled) return;
        if (meta.PacketSize <= NetworkRules.MaxPacketSize) return;
        log.Warn($"Found RPC exceeding maximum packet size ({meta.PacketSize} > {NetworkRules.MaxPacketSize}). Full Info={meta}");
    }
}