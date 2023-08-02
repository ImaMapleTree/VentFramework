using System.Linq;
using InnerNet;

namespace VentLib.Utilities;

public static class NetUtils
{
    public static ClientData? GetClient(this PlayerControl player)
    {
        if (player == null) return null;
        var client = AmongUsClient.Instance.allClients.ToArray().FirstOrDefault(cd => cd.Character != null && cd.Character.PlayerId == player.PlayerId);
        return client;
    }
    
    public static int GetClientId(this PlayerControl player)
    {
        if (player == null) return -1;
        var client = player.GetClient();
        return client?.Id ?? -1;
    }
    
    // ReSharper disable once CompareOfFloatsByEqualityOperator
    public static float DeriveDelay(float flatDelay, float multiplier = 0.0003f) => AmongUsClient.Instance.Ping * multiplier + flatDelay;
    
    public static uint HostNetId()
    {
        PlayerControl? host = PlayerControl.AllPlayerControls.ToArray()
            .FirstOrDefault(player => player.GetClientId() == AmongUsClient.Instance.HostId);
        return host == null ? 0 : host.NetId;
    }
}